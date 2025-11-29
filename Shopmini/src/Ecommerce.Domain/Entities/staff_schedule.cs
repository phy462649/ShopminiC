using System;
using System.Collections.Generic;

namespace Ecommerce.Domain.Entities;

public partial class staff_schedule
{
    public long id { get; set; }

    public long staff_id { get; set; }

    /// <summary>
    /// 0=Sun..6=Sat
    /// </summary>
    public sbyte day_of_week { get; set; }

    public TimeOnly start_time { get; set; }

    public TimeOnly end_time { get; set; }

    public bool? is_working { get; set; }

    public DateTime? created_at { get; set; }

    public DateTime? updated_at { get; set; }

    public virtual staff staff { get; set; } = null!;
}
