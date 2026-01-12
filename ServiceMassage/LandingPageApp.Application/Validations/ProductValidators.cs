using FluentValidation;
using LandingPageApp.Application.Dtos;

namespace LandingPageApp.Application.Validations;

public class CreateProductDtoValidator : AbstractValidator<CreateProductDto>
{
    public CreateProductDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Tên sản phẩm không được để trống")
            .MaximumLength(200)
            .WithMessage("Tên sản phẩm không được vượt quá 200 ký tự");

        RuleFor(x => x.Price)
            .GreaterThan(0)
            .WithMessage("Giá phải lớn hơn 0");

        RuleFor(x => x.Stock)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Tồn kho không được âm");

        RuleFor(x => x.CategoryId)
            .GreaterThan(0)
            .WithMessage("CategoryId phải lớn hơn 0");
    }
}

public class UpdateProductDtoValidator : AbstractValidator<UpdateProductDto>
{
    public UpdateProductDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Tên sản phẩm không được để trống")
            .MaximumLength(200)
            .WithMessage("Tên sản phẩm không được vượt quá 200 ký tự");

        RuleFor(x => x.Price)
            .GreaterThan(0)
            .WithMessage("Giá phải lớn hơn 0");

        RuleFor(x => x.Stock)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Tồn kho không được âm");

        RuleFor(x => x.CategoryId)
            .GreaterThan(0)
            .WithMessage("CategoryId phải lớn hơn 0");
    }
}
