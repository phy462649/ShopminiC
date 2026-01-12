using FluentValidation;
using LandingPageApp.Application.Dtos;

namespace LandingPageApp.Application.Validations;

public class CreateRoomDtoValidator : AbstractValidator<CreateRoomDto>
{
    public CreateRoomDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Tên phòng không được để trống")
            .MaximumLength(100)
            .WithMessage("Tên phòng không được vượt quá 100 ký tự");

        RuleFor(x => x.Capacity)
            .GreaterThan(0)
            .WithMessage("Sức chứa phải lớn hơn 0")
            .LessThanOrEqualTo(20)
            .WithMessage("Sức chứa không được vượt quá 20 người");
    }
}

public class UpdateRoomDtoValidator : AbstractValidator<UpdateRoomDto>
{
    public UpdateRoomDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Tên phòng không được để trống")
            .MaximumLength(100)
            .WithMessage("Tên phòng không được vượt quá 100 ký tự");

        RuleFor(x => x.Capacity)
            .GreaterThan(0)
            .WithMessage("Sức chứa phải lớn hơn 0")
            .LessThanOrEqualTo(20)
            .WithMessage("Sức chứa không được vượt quá 20 người");
    }
}
