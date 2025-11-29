using System;
using System.Collections.Generic;

namespace Ecommerce.Domain.Entities;

public partial class product
{
    public long id { get; set; }

    public string name { get; set; } = null!;

    public string? description { get; set; }

    public decimal price { get; set; }

    public int? stock { get; set; }

    public DateTime? created_at { get; set; }

    public DateTime? updated_at { get; set; }

    public virtual ICollection<order_item> order_items { get; set; } = new List<order_item>();
}
