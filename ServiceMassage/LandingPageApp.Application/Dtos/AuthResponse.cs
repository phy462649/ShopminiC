namespace LandingPageApp.Application.Dtos;

public class AuthResponse
{
    public bool Status { get; set; }
    public string Message { get; set; } = string.Empty;
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public UserDetailDTO? User { get; set; }
    public int ExpiresIn { get; set; }
}
