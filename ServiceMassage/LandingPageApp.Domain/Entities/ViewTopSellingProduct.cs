using System;
using System.Collections.Generic;

namespace LandingPageApp.Domain.Entities;

public partial class ViewTopSellingProduct
{
    public long ProductId { get; set; }

    public string ProductName { get; set; } = null!;

    public decimal? TotalSold { get; set; }

    public decimal? Revenue { get; set; }
}
