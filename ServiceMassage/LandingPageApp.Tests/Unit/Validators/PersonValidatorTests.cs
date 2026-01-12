using FluentValidation.TestHelper;
using LandingPageApp.Application.Dtos;
using LandingPageApp.Application.Validations;

namespace LandingPageApp.Tests.Unit.Validators;

public class PersonValidatorTests
{
    private readonly CreatePersonDtoValidator _createValidator;
    private readonly UpdatePersonDtoValidator _updateValidator;

    public PersonValidatorTests()
    {
        _createValidator = new CreatePersonDtoValidator();
        _updateValidator = new UpdatePersonDtoValidator();
    }

    [Fact]
    public void CreateValidator_WithValidData_ShouldNotHaveErrors()
    {
        // Arrange
        var dto = new CreatePersonDto
        {
            Name = "John Doe",
            Username = "johndoe",
            Password = "Password123",
            Email = "john@example.com",
            Phone = "0123456789",
            RoleId = 1
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
        var dto = new CreatePersonDto
        {
            Name = "",
            Username = "johndoe",
            Password = "Password123",
            RoleId = 1
        };

        // Act
        var result = _createValidator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void CreateValidator_WithShortUsername_ShouldHaveError()
    {
        // Arrange
        var dto = new CreatePersonDto
        {
            Name = "John",
            Username = "abc",
            Password = "Password123",
            RoleId = 1
        };

        // Act
        var result = _createValidator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Username);
    }

    [Fact]
    public void CreateValidator_WithInvalidUsernameChars_ShouldHaveError()
    {
        // Arrange
        var dto = new CreatePersonDto
        {
            Name = "John",
            Username = "john@doe",
            Password = "Password123",
            RoleId = 1
        };

        // Act
        var result = _createValidator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Username);
    }

    [Fact]
    public void CreateValidator_WithWeakPassword_ShouldHaveError()
    {
        // Arrange
        var dto = new CreatePersonDto
        {
            Name = "John",
            Username = "johndoe",
            Password = "password",
            RoleId = 1
        };

        // Act
        var result = _createValidator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void CreateValidator_WithShortPassword_ShouldHaveError()
    {
        // Arrange
        var dto = new CreatePersonDto
        {
            Name = "John",
            Username = "johndoe",
            Password = "Ab1",
            RoleId = 1
        };

        // Act
        var result = _createValidator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void CreateValidator_WithInvalidEmail_ShouldHaveError()
    {
        // Arrange
        var dto = new CreatePersonDto
        {
            Name = "John",
            Username = "johndoe",
            Password = "Password123",
            Email = "invalid-email",
            RoleId = 1
        };

        // Act
        var result = _createValidator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void CreateValidator_WithInvalidPhone_ShouldHaveError()
    {
        // Arrange
        var dto = new CreatePersonDto
        {
            Name = "John",
            Username = "johndoe",
            Password = "Password123",
            Phone = "123",
            RoleId = 1
        };

        // Act
        var result = _createValidator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Phone);
    }

    [Fact]
    public void CreateValidator_WithInvalidRoleId_ShouldHaveError()
    {
        // Arrange
        var dto = new CreatePersonDto
        {
            Name = "John",
            Username = "johndoe",
            Password = "Password123",
            RoleId = 0
        };

        // Act
        var result = _createValidator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.RoleId);
    }

    [Fact]
    public void UpdateValidator_WithValidData_ShouldNotHaveErrors()
    {
        // Arrange
        var dto = new UpdatePersonDto
        {
            Name = "John Updated",
            Email = "john.updated@example.com",
            Phone = "0987654321",
            RoleId = 2
        };

        // Act
        var result = _updateValidator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
