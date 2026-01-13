using LandingPageApp.Api.Controllers;
using LandingPageApp.Application.Dtos;
using LandingPageApp.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LandingPageApp.Tests.Unit.Controllers;

/// <summary>
/// Unit tests cho ProductController.
/// Mock IProductService để test controller logic.
/// </summary>
public class ProductControllerTests
{
    private readonly Mock<IProductService> _productServiceMock;
    private readonly ProductController _sut;

    public ProductControllerTests()
    {
        _productServiceMock = new Mock<IProductService>();
        _sut = new ProductController(_productServiceMock.Object);
    }

    #region GetAll Tests

    [Fact]
    public async Task GetAll_ShouldReturnOkWithProducts()
    {
        // Arrange
        var products = new List<ProductDto>
        {
            new() { Id = 1, Name = "Product 1", Price = 100000 },
            new() { Id = 2, Name = "Product 2", Price = 200000 }
        };
        _productServiceMock.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(products);

        // Act
        var result = await _sut.GetAll(CancellationToken.None);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedProducts = okResult.Value.Should().BeAssignableTo<IEnumerable<ProductDto>>().Subject;
        returnedProducts.Should().HaveCount(2);
    }

    #endregion

    #region GetById Tests

    [Fact]
    public async Task GetById_WhenExists_ShouldReturnOkWithProduct()
    {
        // Arrange
        var product = new ProductDto { Id = 1, Name = "Test Product", Price = 150000 };
        _productServiceMock.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        // Act
        var result = await _sut.GetById(1, CancellationToken.None);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedProduct = okResult.Value.Should().BeOfType<ProductDto>().Subject;
        returnedProduct.Name.Should().Be("Test Product");
        returnedProduct.Price.Should().Be(150000);
    }

    [Fact]
    public async Task GetById_WhenNotExists_ShouldReturnNotFound()
    {
        // Arrange
        _productServiceMock.Setup(x => x.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProductDto?)null);

        // Act
        var result = await _sut.GetById(999, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    #endregion

    #region Create Tests

    [Fact]
    public async Task Create_WithValidData_ShouldReturnCreatedAtAction()
    {
        // Arrange
        var createDto = new CreateProductDto 
        { 
            Name = "New Product", 
            Price = 100000, 
            Stock = 50,
            CategoryId = 1
        };
        var createdProduct = new ProductDto 
        { 
            Id = 1, 
            Name = "New Product", 
            Price = 100000 
        };
        
        _productServiceMock.Setup(x => x.CreateAsync(createDto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdProduct);

        // Act
        var result = await _sut.Create(createDto, CancellationToken.None);

        // Assert
        var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdResult.ActionName.Should().Be(nameof(_sut.GetById));
    }

    #endregion

    #region Delete Tests

    [Fact]
    public async Task Delete_WhenExists_ShouldReturnNoContent()
    {
        // Arrange
        _productServiceMock.Setup(x => x.DeleteAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _sut.Delete(1, CancellationToken.None);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task Delete_WhenNotExists_ShouldReturnNotFound()
    {
        // Arrange
        _productServiceMock.Setup(x => x.DeleteAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _sut.Delete(999, CancellationToken.None);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    #endregion

    #region GetByCategory Tests

    [Fact]
    public async Task GetByCategory_ShouldReturnOkWithProducts()
    {
        // Arrange
        var products = new List<ProductDto>
        {
            new() { Id = 1, Name = "Product 1", CategoryId = 1 },
            new() { Id = 2, Name = "Product 2", CategoryId = 1 }
        };
        _productServiceMock.Setup(x => x.GetByCategoryAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(products);

        // Act
        var result = await _sut.GetByCategory(1, CancellationToken.None);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedProducts = okResult.Value.Should().BeAssignableTo<IEnumerable<ProductDto>>().Subject;
        returnedProducts.Should().HaveCount(2);
        returnedProducts.Should().OnlyContain(p => p.CategoryId == 1);
    }

    #endregion
}
