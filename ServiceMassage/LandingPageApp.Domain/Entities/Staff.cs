using System;
using System.Collections.Generic;

namespace LandingPageApp.Domain.Entities;

public partial class Staff
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public string? Specialty { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<StaffSchedule> StaffSchedules { get; set; } = new List<StaffSchedule>();

    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();
}
