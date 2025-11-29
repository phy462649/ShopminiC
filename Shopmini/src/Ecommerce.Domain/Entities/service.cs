using System;
using System.Collections.Generic;

namespace Ecommerce.Domain.Entities;

public partial class service
{
    public long id { get; set; }

    public string name { get; set; } = null!;

    public string? description { get; set; }

    public int duration_minutes { get; set; }

    public decimal price { get; set; }

    public DateTime? created_at { get; set; }

    public DateTime? updated_at { get; set; }

    public virtual ICollection<booking_service> booking_services { get; set; } = new List<booking_service>();
}
