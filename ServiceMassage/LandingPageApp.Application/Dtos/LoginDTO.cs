using System.ComponentModel.DataAnnotations;

namespace LandingPageApp.Application.Dtos;

public class LoginDTO
{
    [Required(ErrorMessage = "Username is required.")]
    [MinLength(5, ErrorMessage = "Username must be at least 5 characters long.")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required.")]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
    public string Password { get; set; } = string.Empty;

    public string? DeviceToken { get; set; }
}
