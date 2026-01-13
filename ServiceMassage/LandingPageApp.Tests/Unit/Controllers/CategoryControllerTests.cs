using LandingPageApp.Api.Controllers;
using LandingPageApp.Application.Dtos;
using LandingPageApp.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LandingPageApp.Tests.Unit.Controllers;

/// <summary>
/// Unit tests cho CategoryController.
/// Mock ICategoryService để test controller logic.
/// </summary>
public class CategoryControllerTests
{
    private readonly Mock<ICategoryService> _categoryServiceMock;
    private readonly CategoryController _sut;

    public CategoryControllerTests()
    {
        _categoryServiceMock = new Mock<ICategoryService>();
        _sut = new CategoryController(_categoryServiceMock.Object);
    }

    #region GetAll Tests

    [Fact]
    public async Task GetAll_ShouldReturnOkWithCategories()
    {
        // Arrange
        var categories = new List<CategoryDTO>
        {
            new() { Id = 1, Name = "Category 1" },
            new() { Id = 2, Name = "Category 2" }
        };
        _categoryServiceMock.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(categories);

        // Act
        var result = await _sut.GetAll(CancellationToken.None);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedCategories = okResult.Value.Should().BeAssignableTo<IEnumerable<CategoryDTO>>().Subject;
        returnedCategories.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetAll_WhenEmpty_ShouldReturnOkWithEmptyList()
    {
        // Arrange
        _categoryServiceMock.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<CategoryDTO>());

        // Act
        var result = await _sut.GetAll(CancellationToken.None);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedCategories = okResult.Value.Should().BeAssignableTo<IEnumerable<CategoryDTO>>().Subject;
        returnedCategories.Should().BeEmpty();
    }

    #endregion

    #region GetById Tests

    [Fact]
    public async Task GetById_WhenExists_ShouldReturnOkWithCategory()
    {
        // Arrange
        var category = new CategoryDTO { Id = 1, Name = "Test Category" };
        _categoryServiceMock.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        // Act
        var result = await _sut.GetById(1, CancellationToken.None);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedCategory = okResult.Value.Should().BeOfType<CategoryDTO>().Subject;
        returnedCategory.Name.Should().Be("Test Category");
    }

    [Fact]
    public async Task GetById_WhenNotExists_ShouldReturnNotFound()
    {
        // Arrange
        _categoryServiceMock.Setup(x => x.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((CategoryDTO?)null);

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
        var createDto = new CreateCategoryDto { Name = "New Category" };
        var createdCategory = new CategoryDTO { Id = 1, Name = "New Category" };
        
        _categoryServiceMock.Setup(x => x.CreateAsync(createDto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdCategory);

        // Act
        var result = await _sut.Create(createDto, CancellationToken.None);

        // Assert
        var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdResult.ActionName.Should().Be(nameof(_sut.GetById));
        createdResult.RouteValues!["id"].Should().Be(1L);
    }

    #endregion

    #region Update Tests

    [Fact]
    public async Task Update_WhenExists_ShouldReturnOkWithUpdatedCategory()
    {
        // Arrange
        var updateDto = new UpdateCategoryDto { Name = "Updated Category" };
        var updatedCategory = new CategoryDTO { Id = 1, Name = "Updated Category" };
        
        _categoryServiceMock.Setup(x => x.UpdateAsync(1, updateDto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(updatedCategory);

        // Act
        var result = await _sut.Update(1, updateDto, CancellationToken.None);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedCategory = okResult.Value.Should().BeOfType<CategoryDTO>().Subject;
        returnedCategory.Name.Should().Be("Updated Category");
    }

    #endregion

    #region Delete Tests

    [Fact]
    public async Task Delete_WhenExists_ShouldReturnNoContent()
    {
        // Arrange
        _categoryServiceMock.Setup(x => x.DeleteAsync(1, It.IsAny<CancellationToken>()))
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
        _categoryServiceMock.Setup(x => x.DeleteAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _sut.Delete(999, CancellationToken.None);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    #endregion
}
