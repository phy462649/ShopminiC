using FluentValidation.TestHelper;
using LandingPageApp.Application.Dtos;
using LandingPageApp.Application.Validations;
using LandingPageApp.Domain.Enums;

namespace LandingPageApp.Tests.Unit.Validators;

public class BookingValidatorTests
{
    private readonly CreateBookingDtoValidator _validator;

    public BookingValidatorTests()
    {
        _validator = new CreateBookingDtoValidator();
    }

    [Fact]
    public void Validate_WithValidData_ShouldNotHaveErrors()
    {
        // Arrange
        var dto = new CreateBookingDto
        {
            CustomerId = 1,
            StaffId = 1,
            RoomId = 1,
            StartTime = DateTime.UtcNow.AddHours(1),
            EndTime = DateTime.UtcNow.AddHours(2),
            Services = new List<CreateBookingServiceItemDto>
            {
                new() { ServiceId = 1, Quantity = 1 }
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
        var dto = new CreateBookingDto
        {
            CustomerId = 0,
            StaffId = 1,
            RoomId = 1,
            StartTime = DateTime.UtcNow.AddHours(1),
            EndTime = DateTime.UtcNow.AddHours(2),
            Services = new List<CreateBookingServiceItemDto> { new() { ServiceId = 1, Quantity = 1 } }
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.CustomerId);
    }

    [Fact]
    public void Validate_WithPastStartTime_ShouldHaveError()
    {
        // Arrange
        var dto = new CreateBookingDto
        {
            CustomerId = 1,
            StaffId = 1,
            RoomId = 1,
            StartTime = DateTime.UtcNow.AddHours(-1),
            EndTime = DateTime.UtcNow.AddHours(1),
            Services = new List<CreateBookingServiceItemDto> { new() { ServiceId = 1, Quantity = 1 } }
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.StartTime);
    }

    [Fact]
    public void Validate_WithEndTimeBeforeStartTime_ShouldHaveError()
    {
        // Arrange
        var dto = new CreateBookingDto
        {
            CustomerId = 1,
            StaffId = 1,
            RoomId = 1,
            StartTime = DateTime.UtcNow.AddHours(2),
            EndTime = DateTime.UtcNow.AddHours(1),
            Services = new List<CreateBookingServiceItemDto> { new() { ServiceId = 1, Quantity = 1 } }
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.EndTime);
    }

    [Fact]
    public void Validate_WithEmptyServices_ShouldHaveError()
    {
        // Arrange
        var dto = new CreateBookingDto
        {
            CustomerId = 1,
            StaffId = 1,
            RoomId = 1,
            StartTime = DateTime.UtcNow.AddHours(1),
            EndTime = DateTime.UtcNow.AddHours(2),
            Services = new List<CreateBookingServiceItemDto>()
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Services);
    }

    [Fact]
    public void Validate_WithCreatePaymentButNoMethod_ShouldHaveError()
    {
        // Arrange
        var dto = new CreateBookingDto
        {
            CustomerId = 1,
            StaffId = 1,
            RoomId = 1,
            StartTime = DateTime.UtcNow.AddHours(1),
            EndTime = DateTime.UtcNow.AddHours(2),
            Services = new List<CreateBookingServiceItemDto> { new() { ServiceId = 1, Quantity = 1 } },
            CreatePayment = true,
            PaymentMethod = null
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PaymentMethod);
    }

    [Fact]
    public void Validate_WithCreatePaymentAndMethod_ShouldNotHaveError()
    {
        // Arrange
        var dto = new CreateBookingDto
        {
            CustomerId = 1,
            StaffId = 1,
            RoomId = 1,
            StartTime = DateTime.UtcNow.AddHours(1),
            EndTime = DateTime.UtcNow.AddHours(2),
            Services = new List<CreateBookingServiceItemDto> { new() { ServiceId = 1, Quantity = 1 } },
            CreatePayment = true,
            PaymentMethod = PaymentMethod.Cash
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.PaymentMethod);
    }
}
