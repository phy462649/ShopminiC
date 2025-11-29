using System;
using System.Collections.Generic;

namespace Ecommerce.Domain.Entities;

public partial class booking
{
    public long id { get; set; }

    public long customer_id { get; set; }

    public long staff_id { get; set; }

    public long room_id { get; set; }

    public DateTime start_time { get; set; }

    public string? status { get; set; }

    public DateTime? created_at { get; set; }

    public DateTime? updated_at { get; set; }

    public DateTime? end_time { get; set; }

    public virtual ICollection<booking_service> booking_services { get; set; } = new List<booking_service>();

    public virtual customer customer { get; set; } = null!;

    public virtual ICollection<payment> payments { get; set; } = new List<payment>();

    public virtual room room { get; set; } = null!;

    public virtual staff staff { get; set; } = null!;
}
