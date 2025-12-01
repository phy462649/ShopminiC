using System;
using System.Collections.Generic;

namespace LandingPageApp.Domain.Entities;

public partial class BookingService
{
    public long Id { get; set; }

    public long BookingId { get; set; }

    public long ServiceId { get; set; }

    public decimal Price { get; set; }

    public int? Quantity { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Booking Booking { get; set; } = null!;

    public virtual Service Service { get; set; } = null!;
}
