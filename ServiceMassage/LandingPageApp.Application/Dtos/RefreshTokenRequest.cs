namespace LandingPageApp.Application.Dtos;

/// <summary>
/// DTO cho request refresh token.
/// </summary>
public class RefreshTokenRequest
{
    /// <summary>
    /// Refresh token hiện tại.
    /// </summary>
    public string RefreshToken { get; set; } = string.Empty;

    /// <summary>
    /// Device token để xác định thiết bị.
    /// </summary>
    public string DeviceToken { get; set; } = string.Empty;
}
