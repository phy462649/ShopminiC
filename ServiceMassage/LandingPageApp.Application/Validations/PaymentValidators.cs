using FluentValidation;
using LandingPageApp.Application.Dtos;

namespace LandingPageApp.Application.Validations;

public class CreatePaymentDtoValidator : AbstractValidator<CreatePaymentDto>
{
    public CreatePaymentDtoValidator()
    {
        RuleFor(x => x.PaymentType)
            .IsInEnum()
            .WithMessage("Loại thanh toán không hợp lệ");

        RuleFor(x => x.Method)
            .IsInEnum()
            .WithMessage("Phương thức thanh toán không hợp lệ");

        RuleFor(x => x.PersonalId)
            .GreaterThan(0)
            .WithMessage("PersonalId phải lớn hơn 0");

        // Must have either BookingId or OrderId
        RuleFor(x => x)
            .Must(x => x.BookingId.HasValue || x.OrderId.HasValue)
            .WithMessage("Phải có BookingId hoặc OrderId");

        // Cannot have both BookingId and OrderId
        RuleFor(x => x)
            .Must(x => !(x.BookingId.HasValue && x.OrderId.HasValue))
            .WithMessage("Không thể có cả BookingId và OrderId cùng lúc");
    }
}

public class UpdatePaymentStatusDtoValidator : AbstractValidator<UpdatePaymentStatusDto>
{
    public UpdatePaymentStatusDtoValidator()
    {
        RuleFor(x => x.Status)
            .IsInEnum()
            .WithMessage("Trạng thái thanh toán không hợp lệ");
    }
}
