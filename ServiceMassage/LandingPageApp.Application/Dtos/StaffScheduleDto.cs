using LandingPageApp.Domain.Enums;

namespace LandingPageApp.Application.Dtos;

#region Response DTOs

public class StaffScheduleDto
{
    public long Id { get; set; }
    public long StaffId { get; set; }
    public string? StaffName { get; set; }
    public sbyte DayOfWeek { get; set; }
    public string DayOfWeekName => GetDayName(DayOfWeek);
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public ShiftName? ShiftName { get; set; }
    public bool? IsWorking { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    private static string GetDayName(sbyte day) => day switch
    {
        0 => "Chủ nhật",
        1 => "Thứ hai",
        2 => "Thứ ba",
        3 => "Thứ tư",
        4 => "Thứ năm",
        5 => "Thứ sáu",
        6 => "Thứ bảy",
        _ => "Unknown"
    };
}

public class StaffWeeklyScheduleDto
{
    public long StaffId { get; set; }
    public string? StaffName { get; set; }
    public List<StaffScheduleDto> Schedules { get; set; } = new();
}

#endregion

#region Request DTOs

public class CreateStaffScheduleDto
{
    public long StaffId { get; set; }
    public sbyte DayOfWeek { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public ShiftName ShiftName { get; set; }
    public bool IsWorking { get; set; } = true;
}

public class UpdateStaffScheduleDto
{
    public sbyte DayOfWeek { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public ShiftName ShiftName { get; set; }
    public bool IsWorking { get; set; }
}

public class CreateBulkStaffScheduleDto
{
    public long StaffId { get; set; }
    public List<ScheduleItemDto> Schedules { get; set; } = new();
}

public class ScheduleItemDto
{
    public sbyte DayOfWeek { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public ShiftName ShiftName { get; set; }
    public bool IsWorking { get; set; } = true;
}

#endregion
