using AutoMapper;
using LandingPageApp.Application.Dtos;
using LandingPageApp.Application.Exceptions;
using LandingPageApp.Application.Mappings;
using LandingPageApp.Application.Services;
using LandingPageApp.Domain.Entities;
using LandingPageApp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace LandingPageApp.Tests.Unit.Services;

public class ProductServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<ProductService>> _loggerMock;
    private readonly IMapper _mapper;
    private readonly ProductService _sut;

    public ProductServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<ProductService>>();

        var config = new MapperConfiguration(cfg => cfg.AddProfile<ProductMapper>());
        _mapper = config.CreateMapper();

        _sut = new ProductService(_unitOfWorkMock.Object, _mapper, _loggerMock.Object);
    }

    [Fact]
    public async Task UpdateStockAsync_WhenNotExists_ShouldThrowNotFoundException()
    {
        // Arrange
        _unitOfWorkMock.Setup(x => x.products.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _sut.UpdateStockAsync(999, 10));
    }

    [Fact]
    public async Task UpdateStockAsync_WhenInsufficientStock_ShouldThrowBusinessException()
    {
        // Arrange
        var product = new Product { Id = 1, Name = "Test", Stock = 5, Price = 100 };
        _unitOfWorkMock.Setup(x => x.products.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        // Act & Assert
        await Assert.ThrowsAsync<BusinessException>(() => _sut.UpdateStockAsync(1, -10));
    }

    [Fact]
    public async Task DeleteAsync_WhenNotExists_ShouldReturnFalse()
    {
        // Arrange
        _unitOfWorkMock.Setup(x => x.products.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        // Act
        var result = await _sut.DeleteAsync(999);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteAsync_WhenHasOrderItems_ShouldThrowBusinessException()
    {
        // Arrange
        var product = new Product { Id = 1, Name = "Test", Price = 100 };
        _unitOfWorkMock.Setup(x => x.products.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);
        _unitOfWorkMock.Setup(x => x.orderItem.ExistsAsync(It.IsAny<Expression<Func<OrderItem, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<BusinessException>(() => _sut.DeleteAsync(1));
    }

    [Fact]
    public async Task DeleteAsync_WhenNoOrderItems_ShouldReturnTrue()
    {
        // Arrange
        var product = new Product { Id = 1, Name = "Test", Price = 100 };
        _unitOfWorkMock.Setup(x => x.products.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);
        _unitOfWorkMock.Setup(x => x.orderItem.ExistsAsync(It.IsAny<Expression<Func<OrderItem, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _sut.DeleteAsync(1);

        // Assert
        result.Should().BeTrue();
        _unitOfWorkMock.Verify(x => x.products.Delete(product), Times.Once);
    }

    [Fact]
    public async Task UpdateStockAsync_WhenSufficientStock_ShouldUpdateStock()
    {
        // Arrange
        var product = new Product { Id = 1, Name = "Test", Stock = 10, Price = 100 };
        _unitOfWorkMock.Setup(x => x.products.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);
        
        var products = new List<Product> { product }.AsQueryable();
        var mockDbSet = CreateMockDbSet(products);
        _unitOfWorkMock.Setup(x => x.products.Query()).Returns(mockDbSet.Object);

        // Act
        var result = await _sut.UpdateStockAsync(1, -5);

        // Assert
        product.Stock.Should().Be(5);
        _unitOfWorkMock.Verify(x => x.products.Update(product), Times.Once);
    }

    private static Mock<DbSet<T>> CreateMockDbSet<T>(IQueryable<T> data) where T : class
    {
        var mockSet = new Mock<DbSet<T>>();
        mockSet.As<IAsyncEnumerable<T>>()
            .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
            .Returns(new TestAsyncEnumerator<T>(data.GetEnumerator()));
        mockSet.As<IQueryable<T>>()
            .Setup(m => m.Provider)
            .Returns(new TestAsyncQueryProvider<T>(data.Provider));
        mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(data.Expression);
        mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(data.ElementType);
        mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
        return mockSet;
    }
}

// Helper classes for async EF Core mocking
internal class TestAsyncQueryProvider<TEntity> : IAsyncQueryProvider
{
    private readonly IQueryProvider _inner;

    internal TestAsyncQueryProvider(IQueryProvider inner)
    {
        _inner = inner;
    }

    public IQueryable CreateQuery(Expression expression)
    {
        return new TestAsyncEnumerable<TEntity>(expression);
    }

    public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
    {
        return new TestAsyncEnumerable<TElement>(expression);
    }

    public object? Execute(Expression expression)
    {
        return _inner.Execute(expression);
    }

    public TResult Execute<TResult>(Expression expression)
    {
        return _inner.Execute<TResult>(expression);
    }

    public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default)
    {
        var resultType = typeof(TResult).GetGenericArguments()[0];
        var executionResult = typeof(IQueryProvider)
            .GetMethod(
                name: nameof(IQueryProvider.Execute),
                genericParameterCount: 1,
                types: new[] { typeof(Expression) })!
            .MakeGenericMethod(resultType)
            .Invoke(this, new[] { expression });

        return (TResult)typeof(Task).GetMethod(nameof(Task.FromResult))!
            .MakeGenericMethod(resultType)
            .Invoke(null, new[] { executionResult })!;
    }
}

internal class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
{
    public TestAsyncEnumerable(IEnumerable<T> enumerable)
        : base(enumerable)
    { }

    public TestAsyncEnumerable(Expression expression)
        : base(expression)
    { }

    public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        return new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
    }

    IQueryProvider IQueryable.Provider => new TestAsyncQueryProvider<T>(this);
}

internal class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
{
    private readonly IEnumerator<T> _inner;

    public TestAsyncEnumerator(IEnumerator<T> inner)
    {
        _inner = inner;
    }

    public T Current => _inner.Current;

    public ValueTask<bool> MoveNextAsync()
    {
        return ValueTask.FromResult(_inner.MoveNext());
    }

    public ValueTask DisposeAsync()
    {
        _inner.Dispose();
        return ValueTask.CompletedTask;
    }
}
