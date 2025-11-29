using System;
using System.Collections.Generic;

namespace Ecommerce.Domain.Entities;

public partial class order
{
    public long id { get; set; }

    public long customer_id { get; set; }

    public DateTime order_time { get; set; }

    public string? status { get; set; }

    public DateTime? created_at { get; set; }

    public DateTime? updated_at { get; set; }

    public virtual customer customer { get; set; } = null!;

    public virtual ICollection<order_item> order_items { get; set; } = new List<order_item>();

    public virtual ICollection<payment> payments { get; set; } = new List<payment>();
}
