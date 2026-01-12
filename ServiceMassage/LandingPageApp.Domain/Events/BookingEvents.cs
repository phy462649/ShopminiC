namespace LandingPageApp.Domain.Events;

/// <summary>
/// Event khi booking được tạo mới
/// </summary>
public class BookingCreatedEvent : DomainEvent
{
    public long BookingId { get; }
    public long CustomerId { get; }
    public long? StaffId { get; }
    public DateTime BookingDate { get; }
    public string? CustomerEmail { get; }
    public string? CustomerPhone { get; }

    public BookingCreatedEvent(long bookingId, long customerId, long? staffId, 
        DateTime bookingDate, string? customerEmail, string? customerPhone)
    {
        BookingId = bookingId;
        CustomerId = customerId;
        StaffId = staffId;
        BookingDate = bookingDate;
        CustomerEmail = customerEmail;
        CustomerPhone = customerPhone;
    }
}

/// <summary>
/// Event khi booking được xác nhận
/// </summary>
public class BookingConfirmedEvent : DomainEvent
{
    public long BookingId { get; }
    public long CustomerId { get; }
    public string? CustomerEmail { get; }
    public DateTime BookingDate { get; }

    public BookingConfirmedEvent(long bookingId, long customerId, string? customerEmail, DateTime bookingDate)
    {
        BookingId = bookingId;
        CustomerId = customerId;
        CustomerEmail = customerEmail;
        BookingDate = bookingDate;
    }
}

/// <summary>
/// Event khi booking bị hủy
/// </summary>
public class BookingCancelledEvent : DomainEvent
{
    public long BookingId { get; }
    public long CustomerId { get; }
    public string? CustomerEmail { get; }
    public string? CancellationReason { get; }

    public BookingCancelledEvent(long bookingId, long customerId, string? customerEmail, string? reason)
    {
        BookingId = bookingId;
        CustomerId = customerId;
        CustomerEmail = customerEmail;
        CancellationReason = reason;
    }
}

/// <summary>
/// Event khi booking hoàn thành
/// </summary>
public class BookingCompletedEvent : DomainEvent
{
    public long BookingId { get; }
    public long CustomerId { get; }
    public long? StaffId { get; }
    public decimal TotalAmount { get; }

    public BookingCompletedEvent(long bookingId, long customerId, long? staffId, decimal totalAmount)
    {
        BookingId = bookingId;
        CustomerId = customerId;
        StaffId = staffId;
        TotalAmount = totalAmount;
    }
}
