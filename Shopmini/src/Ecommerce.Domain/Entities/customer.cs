using System;
using System.Collections.Generic;

namespace Ecommerce.Domain.Entities;

public partial class customer
{
    public long id { get; set; }

    public string name { get; set; } = null!;

    public string? email { get; set; }

    public string? phone { get; set; }

    public string? address { get; set; }

    public DateTime? created_at { get; set; }

    public DateTime? updated_at { get; set; }

    public virtual ICollection<booking> bookings { get; set; } = new List<booking>();

    public virtual ICollection<order> orders { get; set; } = new List<order>();
}
