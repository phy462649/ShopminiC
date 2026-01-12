namespace LandingPageApp.Application.Dtos;

public class StaffDTO
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? Position { get; set; }
}
