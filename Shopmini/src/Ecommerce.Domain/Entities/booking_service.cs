using System;
using System.Collections.Generic;

namespace Ecommerce.Domain.Entities;

public partial class booking_service
{
    public long id { get; set; }

    public long booking_id { get; set; }

    public long service_id { get; set; }

    public decimal price { get; set; }

    public int? quantity { get; set; }

    public DateTime? updated_at { get; set; }

    public virtual booking booking { get; set; } = null!;

    public virtual service service { get; set; } = null!;
}
