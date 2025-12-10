using System;
using System.Collections.Generic;
using LandingPageApp.Domain.Enums;

namespace LandingPageApp.Domain.Entities;

public partial class Booking
{
    public long Id { get; set; }

    public long CustomerId { get; set; }

    public long StaffId { get; set; }

    public long RoomId { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public StatusBooking? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public decimal? TotalAmount { get; set; }

    public virtual ICollection<BookingService> BookingServices { get; set; } = new List<BookingService>();

    public virtual Person Customer { get; set; } = null!;

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual Room Room { get; set; } = null!;

    public virtual Person Staff { get; set; } = null!;
}
