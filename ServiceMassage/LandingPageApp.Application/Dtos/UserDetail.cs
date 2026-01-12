namespace LandingPageApp.Application.Dtos;

public class UserDetail
{
    public long Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public bool Status { get; set; }

    // Nếu là customer
    public CustomerDTO? Customer { get; set; }

    // Nếu là staff
    public StaffDTO? Staff { get; set; }
}
