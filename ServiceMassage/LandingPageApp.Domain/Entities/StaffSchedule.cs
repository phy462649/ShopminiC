using System;
using System.Collections.Generic;

namespace LandingPageApp.Domain.Entities;

public partial class StaffSchedule
{
    public long Id { get; set; }

    public long StaffId { get; set; }

    /// <summary>
    /// 0=Sun..6=Sat
    /// </summary>
    public sbyte DayOfWeek { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public bool? IsWorking { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Staff Staff { get; set; } = null!;
}
