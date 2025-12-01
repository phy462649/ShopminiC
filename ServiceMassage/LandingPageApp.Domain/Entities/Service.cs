using System;
using System.Collections.Generic;

namespace LandingPageApp.Domain.Entities;

public partial class Service
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public int DurationMinutes { get; set; }

    public decimal Price { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<BookingService> BookingServices { get; set; } = new List<BookingService>();
}
