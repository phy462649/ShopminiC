using System;
using System.Collections.Generic;

namespace Ecommerce.Domain.Entities;

public partial class order_item
{
    public long id { get; set; }

    public long order_id { get; set; }

    public long product_id { get; set; }

    public int quantity { get; set; }

    public decimal price { get; set; }

    public DateTime? updated_at { get; set; }

    public virtual order order { get; set; } = null!;

    public virtual product product { get; set; } = null!;
}
