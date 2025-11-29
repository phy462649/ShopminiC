using System;
using System.Collections.Generic;

namespace Ecommerce.Domain.Entities;

public partial class view_top_selling_product
{
    public long product_id { get; set; }

    public string product_name { get; set; } = null!;

    public decimal? total_sold { get; set; }

    public decimal? revenue { get; set; }
}
