using LandingPageApp.Domain.Enums;
using System;
using System.Collections.Generic;

namespace LandingPageApp.Domain.Entities;

public partial class Order
{
    public long Id { get; set; }

    public long CustomerId { get; set; }

    public DateTime OrderTime { get; set; }

    public OrderStatus? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public decimal? TotalAmount { get; set; }

    public virtual Person Customer { get; set; } = null!;

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
