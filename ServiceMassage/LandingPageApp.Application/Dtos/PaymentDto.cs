using LandingPageApp.Domain.Enums;

namespace LandingPageApp.Application.Dtos;

#region Response DTOs

public class PaymentDto
{
    public long Id { get; set; }
    public Payment_type? PaymentType { get; set; }
    public decimal Amount { get; set; }
    public PaymentMethod? Method { get; set; }
    public PaymentStatus? Status { get; set; }
    public DateTime? PaymentTime { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    // Booking info
    public long? BookingId { get; set; }
    public string? BookingStatus { get; set; }
    public DateTime? BookingDate { get; set; }
    
    // Order info
    public long? OrderId { get; set; }
    public string? OrderStatus { get; set; }
    public decimal? OrderTotalAmount { get; set; }
    
    // Person info
    public long PersonalId { get; set; }
    public string? PersonalName { get; set; }
    public string? PersonalPhone { get; set; }
    public string? PersonalEmail { get; set; }
    
    // public string? CreatedBy { get; set; }
}

#endregion

#region Request DTOs

public class CreatePaymentDto
{
    public Payment_type PaymentType { get; set; }
    public long? BookingId { get; set; }
    public long? OrderId { get; set; }
    public long PersonalId { get; set; }
    public PaymentMethod Method { get; set; }
    // public string? CreatedBy { get; set; }
}

public class UpdatePaymentStatusDto
{
    public PaymentStatus Status { get; set; }
}

#endregion
