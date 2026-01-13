namespace LandingPageApp.Application.Dtos;

/// <summary>
/// DTO chứa thông tin đăng nhập bằng Google.
/// </summary>
public class GoogleLoginDto
{
    /// <summary>
    /// ID Token từ Google Sign-In (JWT token từ Google).
    /// </summary>
    public string IdToken { get; set; } = string.Empty;

    /// <summary>
    /// Mã định danh thiết bị (tùy chọn).
    /// </summary>
    public string? DeviceToken { get; set; }
}

/// <summary>
/// Thông tin người dùng từ Google.
/// </summary>
public class GoogleUserInfo
{
    /// <summary>
    /// Google User ID (sub claim).
    /// </summary>
    public string GoogleId { get; set; } = string.Empty;

    /// <summary>
    /// Email của người dùng.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Tên đầy đủ của người dùng.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Tên (first name).
    /// </summary>
    public string? GivenName { get; set; }

    /// <summary>
    /// Họ (family name).
    /// </summary>
    public string? FamilyName { get; set; }

    /// <summary>
    /// URL ảnh đại diện.
    /// </summary>
    public string? Picture { get; set; }

    /// <summary>
    /// Email đã được xác thực hay chưa.
    /// </summary>
    public bool EmailVerified { get; set; }
}
