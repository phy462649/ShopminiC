using System.ComponentModel.DataAnnotations;

namespace LandingPageApp.Application.Dtos;

public class ResendOtpRequest
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; } = string.Empty;
}
