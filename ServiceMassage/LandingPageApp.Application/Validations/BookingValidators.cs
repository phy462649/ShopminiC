using FluentValidation;
using LandingPageApp.Application.Dtos;

namespace LandingPageApp.Application.Validations;

public class CreateBookingDtoValidator : AbstractValidator<CreateBookingDto>
{
    public CreateBookingDtoValidator()
    {
        RuleFor(x => x.CustomerId)
            .GreaterThan(0)
            .WithMessage("CustomerId phải lớn hơn 0");

        RuleFor(x => x.StaffId)
            .GreaterThan(0)
            .WithMessage("StaffId phải lớn hơn 0");

        RuleFor(x => x.RoomId)
            .GreaterThan(0)
            .WithMessage("RoomId phải lớn hơn 0");

        RuleFor(x => x.StartTime)
            .GreaterThan(DateTime.UtcNow.AddMinutes(-5))
            .WithMessage("Thời gian bắt đầu phải trong tương lai");

        RuleFor(x => x.EndTime)
            .GreaterThan(x => x.StartTime)
            .WithMessage("Thời gian kết thúc phải sau thời gian bắt đầu");

        RuleFor(x => x.Services)
            .NotEmpty()
            .WithMessage("Phải chọn ít nhất một dịch vụ");

        RuleForEach(x => x.Services).ChildRules(service =>
        {
            service.RuleFor(s => s.ServiceId)
                .GreaterThan(0)
                .WithMessage("ServiceId phải lớn hơn 0");

            service.RuleFor(s => s.Quantity)
                .GreaterThan(0)
                .WithMessage("Số lượng phải lớn hơn 0");
        });

        When(x => x.CreatePayment, () =>
        {
            RuleFor(x => x.PaymentMethod)
                .NotNull()
                .WithMessage("Phải chọn phương thức thanh toán khi tạo payment");
        });
    }
}

public class UpdateBookingDtoValidator : AbstractValidator<UpdateBookingDto>
{
    public UpdateBookingDtoValidator()
    {
        RuleFor(x => x.StaffId)
            .GreaterThan(0)
            .WithMessage("StaffId phải lớn hơn 0");

        RuleFor(x => x.RoomId)
            .GreaterThan(0)
            .WithMessage("RoomId phải lớn hơn 0");

        RuleFor(x => x.EndTime)
            .GreaterThan(x => x.StartTime)
            .WithMessage("Thời gian kết thúc phải sau thời gian bắt đầu");
    }
}
