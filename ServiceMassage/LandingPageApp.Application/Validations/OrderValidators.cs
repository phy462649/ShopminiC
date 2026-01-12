using FluentValidation;
using LandingPageApp.Application.Dtos;

namespace LandingPageApp.Application.Validations;

public class CreateOrderDtoValidator : AbstractValidator<CreateOrderDto>
{
    public CreateOrderDtoValidator()
    {
        RuleFor(x => x.CustomerId)
            .GreaterThan(0)
            .WithMessage("CustomerId phải lớn hơn 0");

        RuleFor(x => x.Items)
            .NotEmpty()
            .WithMessage("Đơn hàng phải có ít nhất một sản phẩm");

        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.ProductId)
                .GreaterThan(0)
                .WithMessage("ProductId phải lớn hơn 0");

            item.RuleFor(i => i.Quantity)
                .GreaterThan(0)
                .WithMessage("Số lượng phải lớn hơn 0")
                .LessThanOrEqualTo(100)
                .WithMessage("Số lượng không được vượt quá 100");
        });

        When(x => x.CreatePayment, () =>
        {
            RuleFor(x => x.PaymentMethod)
                .NotNull()
                .WithMessage("Phải chọn phương thức thanh toán khi tạo payment");
        });
    }
}
