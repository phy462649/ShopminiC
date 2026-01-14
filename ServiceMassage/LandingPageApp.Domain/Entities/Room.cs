using System;
using System.Collections.Generic;

namespace LandingPageApp.Domain.Entities;

public partial class Room
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public int? Capacity { get; set; }

    public bool Active { get; set; } = true;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
