using LandingPageApp.Domain.Enums;

namespace LandingPageApp.Application.Dtos;

#region Response DTOs

public class BookingDto
{
    public long Id { get; set; }
    public long CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public long StaffId { get; set; }
    public string? StaffName { get; set; }
    public long RoomId { get; set; }
    public string? RoomName { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public StatusBooking? Status { get; set; }
    public decimal? TotalAmount { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<BookingServiceItemDto> Services { get; set; } = new();
}

public class BookingServiceItemDto
{
    public long Id { get; set; }
    public long ServiceId { get; set; }
    public string? ServiceName { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public decimal Subtotal => Price * Quantity;
}

#endregion

#region Request DTOs

public class CreateBookingDto
{
    public long CustomerId { get; set; }
    public long StaffId { get; set; }
    public long RoomId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public List<CreateBookingServiceItemDto> Services { get; set; } = new();
    public bool CreatePayment { get; set; } = false;
    public PaymentMethod? PaymentMethod { get; set; }
}

public class CreateBookingServiceItemDto
{
    public long ServiceId { get; set; }
    public int Quantity { get; set; } = 1;
}

public class UpdateBookingDto
{
    public long StaffId { get; set; }
    public long RoomId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public StatusBooking Status { get; set; }
}

public class UpdateBookingStatusDto
{
    public StatusBooking Status { get; set; }
}

#endregion
