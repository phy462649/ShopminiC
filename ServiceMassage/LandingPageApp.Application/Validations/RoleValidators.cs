using FluentValidation;
using LandingPageApp.Application.Dtos;

namespace LandingPageApp.Application.Validations;

public class CreateRoleDtoValidator : AbstractValidator<CreateRoleDto>
{
    public CreateRoleDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Tên role không được để trống")
            .MaximumLength(50)
            .WithMessage("Tên role không được vượt quá 50 ký tự")
            .Matches("^[A-Z_]+$")
            .WithMessage("Tên role chỉ được chứa chữ in hoa và dấu gạch dưới");
    }
}

public class UpdateRoleDtoValidator : AbstractValidator<UpdateRoleDto>
{
    public UpdateRoleDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Tên role không được để trống")
            .MaximumLength(50)
            .WithMessage("Tên role không được vượt quá 50 ký tự")
            .Matches("^[A-Z_]+$")
            .WithMessage("Tên role chỉ được chứa chữ in hoa và dấu gạch dưới");
    }
}
