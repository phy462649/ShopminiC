using System.ComponentModel.DataAnnotations;

namespace LandingPageApp.Application.Dtos;

public class EmailVerificationRequest
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "OTP is required")]
    public string Otp { get; set; } = string.Empty;
}
