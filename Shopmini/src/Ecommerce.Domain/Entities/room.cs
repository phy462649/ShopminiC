using System;
using System.Collections.Generic;

namespace Ecommerce.Domain.Entities;

public partial class room
{
    public long id { get; set; }

    public string name { get; set; } = null!;

    public string? description { get; set; }

    public int? capacity { get; set; }

    public DateTime? created_at { get; set; }

    public DateTime? updated_at { get; set; }

    public virtual ICollection<booking> bookings { get; set; } = new List<booking>();
}
