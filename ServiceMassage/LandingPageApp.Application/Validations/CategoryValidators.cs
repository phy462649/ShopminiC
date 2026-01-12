using FluentValidation;
using LandingPageApp.Application.Dtos;

namespace LandingPageApp.Application.Validations;

public class CreateCategoryDtoValidator : AbstractValidator<CreateCategoryDto>
{
    public CreateCategoryDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Tên danh mục không được để trống")
            .MaximumLength(100)
            .WithMessage("Tên danh mục không được vượt quá 100 ký tự");
    }
}

public class UpdateCategoryDtoValidator : AbstractValidator<UpdateCategoryDto>
{
    public UpdateCategoryDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Tên danh mục không được để trống")
            .MaximumLength(100)
            .WithMessage("Tên danh mục không được vượt quá 100 ký tự");
    }
}
