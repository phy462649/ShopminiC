namespace LandingPageApp.Application.Dtos;

#region Response DTOs

public class ServiceDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int DurationMinutes { get; set; }
    public decimal Price { get; set; }
    public string? UrlImage { get; set; }
}

#endregion

#region Request DTOs

public class CreateServiceDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int DurationMinutes { get; set; }
    public decimal Price { get; set; }
    public string? UrlImage { get; set; }
}

public class UpdateServiceDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int DurationMinutes { get; set; }
    public decimal Price { get; set; }
    public string? UrlImage { get; set; }
}

#endregion
