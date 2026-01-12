using System;
using System.Collections.Generic;
using LandingPageApp.Domain.Enums;
using LandingPageApp.Domain.Events;

namespace LandingPageApp.Domain.Entities;

public partial class Booking : BaseEntity
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

    #region Domain Events Methods

    /// <summary>
    /// Raise event khi booking được tạo
    /// </summary>
    public void RaiseBookingCreatedEvent(string? customerEmail, string? customerPhone)
    {
        AddDomainEvent(new BookingCreatedEvent(
            Id, CustomerId, StaffId, StartTime, customerEmail, customerPhone));
    }

    /// <summary>
    /// Raise event khi booking được xác nhận
    /// </summary>
    public void RaiseBookingConfirmedEvent(string? customerEmail)
    {
        AddDomainEvent(new BookingConfirmedEvent(Id, CustomerId, customerEmail, StartTime));
    }

    /// <summary>
    /// Raise event khi booking bị hủy
    /// </summary>
    public void RaiseBookingCancelledEvent(string? customerEmail, string? reason = null)
    {
        AddDomainEvent(new BookingCancelledEvent(Id, CustomerId, customerEmail, reason));
    }

    /// <summary>
    /// Raise event khi booking hoàn thành
    /// </summary>
    public void RaiseBookingCompletedEvent()
    {
        AddDomainEvent(new BookingCompletedEvent(Id, CustomerId, StaffId, TotalAmount ?? 0));
    }

    #endregion
}
