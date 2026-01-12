using FluentValidation.TestHelper;
using LandingPageApp.Application.Dtos;
using LandingPageApp.Application.Validations;
using LandingPageApp.Domain.Enums;

namespace LandingPageApp.Tests.Unit.Validators;

public class OrderValidatorTests
{
    private readonly CreateOrderDtoValidator _validator;

    public OrderValidatorTests()
    {
        _validator = new CreateOrderDtoValidator();
    }

    [Fact]
    public void Validate_WithValidData_ShouldNotHaveErrors()
    {
        // Arrange
        var dto = new CreateOrderDto
        {
            CustomerId = 1,
            Items = new List<CreateOrderItemDto>
            {
                new() { ProductId = 1, Quantity = 2 }
            }
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithInvalidCustomerId_ShouldHaveError()
    {
        // Arrange
        var dto = new CreateOrderDto
        {
            CustomerId = 0,
            Items = new List<CreateOrderItemDto> { new() { ProductId = 1, Quantity = 1 } }
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.CustomerId);
    }

    [Fact]
    public void Validate_WithEmptyItems_ShouldHaveError()
    {
        // Arrange
        var dto = new CreateOrderDto
        {
            CustomerId = 1,
            Items = new List<CreateOrderItemDto>()
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Items);
    }

    [Fact]
    public void Validate_WithInvalidProductId_ShouldHaveError()
    {
        // Arrange
        var dto = new CreateOrderDto
        {
            CustomerId = 1,
            Items = new List<CreateOrderItemDto>
            {
                new() { ProductId = 0, Quantity = 1 }
            }
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.Errors.Should().NotBeEmpty();
    }

    [Fact]
    public void Validate_WithZeroQuantity_ShouldHaveError()
    {
        // Arrange
        var dto = new CreateOrderDto
        {
            CustomerId = 1,
            Items = new List<CreateOrderItemDto>
            {
                new() { ProductId = 1, Quantity = 0 }
            }
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.Errors.Should().NotBeEmpty();
    }

    [Fact]
    public void Validate_WithQuantityOver100_ShouldHaveError()
    {
        // Arrange
        var dto = new CreateOrderDto
        {
            CustomerId = 1,
            Items = new List<CreateOrderItemDto>
            {
                new() { ProductId = 1, Quantity = 101 }
            }
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.Errors.Should().NotBeEmpty();
    }

    [Fact]
    public void Validate_WithCreatePaymentButNoMethod_ShouldHaveError()
    {
        // Arrange
        var dto = new CreateOrderDto
        {
            CustomerId = 1,
            Items = new List<CreateOrderItemDto> { new() { ProductId = 1, Quantity = 1 } },
            CreatePayment = true,
            PaymentMethod = null
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PaymentMethod);
    }
}
