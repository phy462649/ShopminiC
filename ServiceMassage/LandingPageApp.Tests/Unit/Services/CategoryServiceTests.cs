using AutoMapper;
using LandingPageApp.Application.Dtos;
using LandingPageApp.Application.Exceptions;
using LandingPageApp.Application.Mappings;
using LandingPageApp.Application.Services;
using LandingPageApp.Domain.Entities;
using LandingPageApp.Domain.Repositories;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace LandingPageApp.Tests.Unit.Services;

public class CategoryServiceTests
{
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
    private readonly Mock<ILogger<CategoryService>> _loggerMock;
    private readonly IMapper _mapper;
    private readonly CategoryService _sut;

    public CategoryServiceTests()
    {
        _categoryRepositoryMock = new Mock<ICategoryRepository>();
        _loggerMock = new Mock<ILogger<CategoryService>>();

        var config = new MapperConfiguration(cfg => cfg.AddProfile<CategoryMapper>());
        _mapper = config.CreateMapper();

        _sut = new CategoryService(_categoryRepositoryMock.Object, _mapper, _loggerMock.Object);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllCategories()
    {
        // Arrange
        var categories = new List<Category>
        {
            new() { Id = 1, Name = "Category 1" },
            new() { Id = 2, Name = "Category 2" }
        };
        _categoryRepositoryMock.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(categories);

        // Act
        var result = await _sut.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetByIdAsync_WhenExists_ShouldReturnCategory()
    {
        // Arrange
        var category = new Category { Id = 1, Name = "Test Category" };
        _categoryRepositoryMock.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        // Act
        var result = await _sut.GetByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Test Category");
    }

    [Fact]
    public async Task CreateAsync_WithValidData_ShouldCreateCategory()
    {
        // Arrange
        var createDto = new CreateCategoryDto { Name = "New Category" };
        _categoryRepositoryMock.Setup(x => x.ExistsAsync(It.IsAny<Expression<Func<Category, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _sut.CreateAsync(createDto);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("New Category");
        _categoryRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithDuplicateName_ShouldThrowBusinessException()
    {
        // Arrange
        var createDto = new CreateCategoryDto { Name = "Existing" };
        _categoryRepositoryMock.Setup(x => x.ExistsAsync(It.IsAny<Expression<Func<Category, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<BusinessException>(() => _sut.CreateAsync(createDto));
    }

    [Fact]
    public async Task UpdateAsync_WhenNotExists_ShouldThrowNotFoundException()
    {
        // Arrange
        var updateDto = new UpdateCategoryDto { Name = "Updated" };
        _categoryRepositoryMock.Setup(x => x.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _sut.UpdateAsync(999, updateDto));
    }

    [Fact]
    public async Task DeleteAsync_WhenHasProducts_ShouldThrowBusinessException()
    {
        // Arrange
        var category = new Category
        {
            Id = 1,
            Name = "Category",
            Products = new List<Product> { new() { Id = 1, Name = "Product" } }
        };
        _categoryRepositoryMock.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        // Act & Assert
        await Assert.ThrowsAsync<BusinessException>(() => _sut.DeleteAsync(1));
    }
}
