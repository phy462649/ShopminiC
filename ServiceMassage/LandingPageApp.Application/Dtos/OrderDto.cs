using LandingPageApp.Domain.Enums;

namespace LandingPageApp.Application.Dtos;

#region Response DTOs

public class OrderDto
{
    public long Id { get; set; }
    public long CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public DateTime OrderTime { get; set; }
    public OrderStatus? Status { get; set; }
    public decimal? TotalAmount { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<OrderItemDto> Items { get; set; } = new();
}

public class OrderItemDto
{
    public long Id { get; set; }
    public long ProductId { get; set; }
    public string? ProductName { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public decimal Subtotal => Price * Quantity;
}

#endregion

#region Request DTOs

public class CreateOrderDto
{
    public long CustomerId { get; set; }
    public List<CreateOrderItemDto> Items { get; set; } = new();
    public bool CreatePayment { get; set; } = false;
    public PaymentMethod? PaymentMethod { get; set; }
}

public class CreateOrderItemDto
{
    public long ProductId { get; set; }
    public int Quantity { get; set; }
}

public class UpdateOrderStatusDto
{
    public OrderStatus Status { get; set; }
}

public class UpdateOrderItemDto
{
    public int Quantity { get; set; }
}

#endregion
