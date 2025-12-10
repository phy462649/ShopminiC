using LandingPageApp.Domain.Enums;
using System;
using System.Collections.Generic;

namespace LandingPageApp.Domain.Entities;

public partial class Payment
{
    public long Id { get; set; }

    public Payment_type? PaymentType { get; set; } = default(Payment_type?);

    public decimal Amount { get; set; }

    public PaymentMethod? Method { get; set; }

    public PaymentStatus? Status { get; set; }

    public DateTime? PaymentTime { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public long? BookingId { get; set; }

    public long? OrderId { get; set; }

    public long PersonalId { get; set; }

    public string? CreatedBy { get; set; }

    public virtual Booking? Booking { get; set; }

    public virtual Order? Order { get; set; }

    public virtual Person Personal { get; set; } = null!;
}
