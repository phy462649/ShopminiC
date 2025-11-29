using System;
using System.Collections.Generic;

namespace Ecommerce.Domain.Entities;

public partial class role
{
    public long id { get; set; }

    public string name { get; set; } = null!;

    public string? description { get; set; }

    public DateTime? created_at { get; set; }

    public DateTime? updated_at { get; set; }

    public virtual ICollection<staff> staff { get; set; } = new List<staff>();
}
