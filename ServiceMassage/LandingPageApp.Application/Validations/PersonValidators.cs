using FluentValidation;
using LandingPageApp.Application.Dtos;

namespace LandingPageApp.Application.Validations;

public class CreatePersonDtoValidator : AbstractValidator<CreatePersonDto>
{
    public CreatePersonDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Tên không được để trống")
            .MaximumLength(100)
            .WithMessage("Tên không được vượt quá 100 ký tự");

        RuleFor(x => x.Username)
            .NotEmpty()
            .WithMessage("Username không được để trống")
            .MinimumLength(4)
            .WithMessage("Username phải có ít nhất 4 ký tự")
            .MaximumLength(30)
            .WithMessage("Username không được vượt quá 30 ký tự")
            .Matches("^[a-zA-Z0-9_]+$")
            .WithMessage("Username chỉ được chứa chữ cái, số và dấu gạch dưới");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password không được để trống")
            .MinimumLength(6)
            .WithMessage("Password phải có ít nhất 6 ký tự")
            .Matches(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d).+$")
            .WithMessage("Password phải chứa chữ hoa, chữ thường và số");

        RuleFor(x => x.Email)
            .EmailAddress()
            .When(x => !string.IsNullOrEmpty(x.Email))
            .WithMessage("Email không hợp lệ");

        RuleFor(x => x.Phone)
            .Matches(@"^\d{9,12}$")
            .When(x => !string.IsNullOrEmpty(x.Phone))
            .WithMessage("Số điện thoại phải có 9-12 chữ số");

        RuleFor(x => x.RoleId)
            .GreaterThan(0)
            .WithMessage("RoleId phải lớn hơn 0");
    }
}

public class UpdatePersonDtoValidator : AbstractValidator<UpdatePersonDto>
{
    public UpdatePersonDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Tên không được để trống")
            .MaximumLength(100)
            .WithMessage("Tên không được vượt quá 100 ký tự");

        RuleFor(x => x.Email)
            .EmailAddress()
            .When(x => !string.IsNullOrEmpty(x.Email))
            .WithMessage("Email không hợp lệ");

        RuleFor(x => x.Phone)
            .Matches(@"^\d{9,12}$")
            .When(x => !string.IsNullOrEmpty(x.Phone))
            .WithMessage("Số điện thoại phải có 9-12 chữ số");

        RuleFor(x => x.RoleId)
            .GreaterThan(0)
            .WithMessage("RoleId phải lớn hơn 0");
    }
}
