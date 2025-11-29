using System;
using System.Collections.Generic;

namespace Ecommerce.Domain.Entities;

public partial class payment
{
    public long id { get; set; }

    public string payment_type { get; set; } = null!;

    public long reference_id { get; set; }

    public decimal amount { get; set; }

    public string? method { get; set; }

    public string? status { get; set; }

    public DateTime? payment_time { get; set; }

    public DateTime? updated_at { get; set; }

    public long? booking_id { get; set; }

    public long? order_id { get; set; }

    public string? created_by { get; set; }

    public virtual booking? booking { get; set; }

    public virtual order? order { get; set; }
}
