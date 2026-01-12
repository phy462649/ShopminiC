using FluentValidation.TestHelper;
using LandingPageApp.Application.Dtos;
using LandingPageApp.Application.Validations;

namespace LandingPageApp.Tests.Unit.Validators;

public class ProductValidatorTests
{
    private readonly CreateProductDtoValidator _createValidator;
    private readonly UpdateProductDtoValidator _updateValidator;

    public ProductValidatorTests()
    {
        _createValidator = new CreateProductDtoValidator();
        _updateValidator = new UpdateProductDtoValidator();
    }

    [Fact]
    public void CreateValidator_WithValidData_ShouldNotHaveErrors()
    {
        // Arrange
        var dto = new CreateProductDto
        {
            Name = "Test Product",
            Price = 100,
            Stock = 10,
            CategoryId = 1
        };

        // Act
        var result = _createValidator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void CreateValidator_WithEmptyName_ShouldHaveError()
    {
        // Arrange
        var dto = new CreateProductDto
        {
            Name = "",
            Price = 100,
            Stock = 10,
            CategoryId = 1
        };

        // Act
        var result = _createValidator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void CreateValidator_WithNameTooLong_ShouldHaveError()
    {
        // Arrange
        var dto = new CreateProductDto
        {
            Name = new string('a', 201),
            Price = 100,
            Stock = 10,
            CategoryId = 1
        };

        // Act
        var result = _createValidator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void CreateValidator_WithZeroPrice_ShouldHaveError()
    {
        // Arrange
        var dto = new CreateProductDto
        {
            Name = "Test",
            Price = 0,
            Stock = 10,
            CategoryId = 1
        };

        // Act
        var result = _createValidator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Price);
    }

    [Fact]
    public void CreateValidator_WithNegativePrice_ShouldHaveError()
    {
        // Arrange
        var dto = new CreateProductDto
        {
            Name = "Test",
            Price = -10,
            Stock = 10,
            CategoryId = 1
        };

        // Act
        var result = _createValidator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Price);
    }

    [Fact]
    public void CreateValidator_WithNegativeStock_ShouldHaveError()
    {
        // Arrange
        var dto = new CreateProductDto
        {
            Name = "Test",
            Price = 100,
            Stock = -1,
            CategoryId = 1
        };

        // Act
        var result = _createValidator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Stock);
    }

    [Fact]
    public void CreateValidator_WithInvalidCategoryId_ShouldHaveError()
    {
        // Arrange
        var dto = new CreateProductDto
        {
            Name = "Test",
            Price = 100,
            Stock = 10,
            CategoryId = 0
        };

        // Act
        var result = _createValidator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.CategoryId);
    }

    [Fact]
    public void UpdateValidator_WithValidData_ShouldNotHaveErrors()
    {
        // Arrange
        var dto = new UpdateProductDto
        {
            Name = "Updated Product",
            Price = 150,
            Stock = 20,
            CategoryId = 2
        };

        // Act
        var result = _updateValidator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
