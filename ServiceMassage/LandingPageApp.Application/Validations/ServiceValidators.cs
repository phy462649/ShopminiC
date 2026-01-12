using FluentValidation;
using LandingPageApp.Application.Dtos;

namespace LandingPageApp.Application.Validations;

public class CreateServiceDtoValidator : AbstractValidator<CreateServiceDto>
{
    public CreateServiceDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Tên dịch vụ không được để trống")
            .MaximumLength(200)
            .WithMessage("Tên dịch vụ không được vượt quá 200 ký tự");

        RuleFor(x => x.DurationMinutes)
            .GreaterThan(0)
            .WithMessage("Thời gian dịch vụ phải lớn hơn 0 phút")
            .LessThanOrEqualTo(480)
            .WithMessage("Thời gian dịch vụ không được vượt quá 8 giờ (480 phút)");

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Giá dịch vụ không được âm");
    }
}

public class UpdateServiceDtoValidator : AbstractValidator<UpdateServiceDto>
{
    public UpdateServiceDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Tên dịch vụ không được để trống")
            .MaximumLength(200)
            .WithMessage("Tên dịch vụ không được vượt quá 200 ký tự");

        RuleFor(x => x.DurationMinutes)
            .GreaterThan(0)
            .WithMessage("Thời gian dịch vụ phải lớn hơn 0 phút")
            .LessThanOrEqualTo(480)
            .WithMessage("Thời gian dịch vụ không được vượt quá 8 giờ (480 phút)");

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Giá dịch vụ không được âm");
    }
}
