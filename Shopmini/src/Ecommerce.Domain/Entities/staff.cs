using System;
using System.Collections.Generic;

namespace Ecommerce.Domain.Entities;

public partial class staff
{
    public long id { get; set; }

    public string name { get; set; } = null!;

    public string? phone { get; set; }

    public string? email { get; set; }

    public string? specialty { get; set; }

    public DateTime? created_at { get; set; }

    public DateTime? updated_at { get; set; }

    public virtual ICollection<booking> bookings { get; set; } = new List<booking>();

    public virtual ICollection<staff_schedule> staff_schedules { get; set; } = new List<staff_schedule>();

    public virtual ICollection<role> roles { get; set; } = new List<role>();
}
