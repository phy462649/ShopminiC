using FluentValidation;
using LandingPageApp.Application.Dtos;

namespace LandingPageApp.Application.Validations;

public class CreateStaffScheduleDtoValidator : AbstractValidator<CreateStaffScheduleDto>
{
    public CreateStaffScheduleDtoValidator()
    {
        RuleFor(x => x.StaffId)
            .GreaterThan(0)
            .WithMessage("StaffId phải lớn hơn 0");

        RuleFor(x => x.DayOfWeek)
            .InclusiveBetween((sbyte)0, (sbyte)6)
            .WithMessage("DayOfWeek phải từ 0 (Chủ nhật) đến 6 (Thứ bảy)");

        RuleFor(x => x.EndTime)
            .GreaterThan(x => x.StartTime)
            .WithMessage("Giờ kết thúc phải sau giờ bắt đầu");

        RuleFor(x => x.ShiftName)
            .IsInEnum()
            .WithMessage("Ca làm việc không hợp lệ");
    }
}

public class UpdateStaffScheduleDtoValidator : AbstractValidator<UpdateStaffScheduleDto>
{
    public UpdateStaffScheduleDtoValidator()
    {
        RuleFor(x => x.DayOfWeek)
            .InclusiveBetween((sbyte)0, (sbyte)6)
            .WithMessage("DayOfWeek phải từ 0 (Chủ nhật) đến 6 (Thứ bảy)");

        RuleFor(x => x.EndTime)
            .GreaterThan(x => x.StartTime)
            .WithMessage("Giờ kết thúc phải sau giờ bắt đầu");

        RuleFor(x => x.ShiftName)
            .IsInEnum()
            .WithMessage("Ca làm việc không hợp lệ");
    }
}

public class CreateBulkStaffScheduleDtoValidator : AbstractValidator<CreateBulkStaffScheduleDto>
{
    public CreateBulkStaffScheduleDtoValidator()
    {
        RuleFor(x => x.StaffId)
            .GreaterThan(0)
            .WithMessage("StaffId phải lớn hơn 0");

        RuleFor(x => x.Schedules)
            .NotEmpty()
            .WithMessage("Phải có ít nhất một lịch làm việc");

        RuleForEach(x => x.Schedules).ChildRules(schedule =>
        {
            schedule.RuleFor(s => s.DayOfWeek)
                .InclusiveBetween((sbyte)0, (sbyte)6)
                .WithMessage("DayOfWeek phải từ 0 đến 6");

            schedule.RuleFor(s => s.EndTime)
                .GreaterThan(s => s.StartTime)
                .WithMessage("Giờ kết thúc phải sau giờ bắt đầu");
        });
    }
}
