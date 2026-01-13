
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LandingPageApp.Application.Dtos;

namespace LandingPageApp.Application.Interfaces
{
    /// <summary>
    /// Interface định nghĩa các chức năng xác thực người dùng.
    /// </summary>
    public interface IAuthService
    {
        Task<AuthResponse> LoginAsync(LoginDTO loginDto, CancellationToken cancellationToken = default);
        Task<AuthResponse> RegisterAsync(RegisterDTO registerDto, CancellationToken cancellationToken = default);
        Task<AuthResponse> RefreshTokenAsync(string refreshToken,string deviceId, CancellationToken cancellationToken = default);
        Task<ApiResponse> LogoutAsync(string refreshToken, CancellationToken cancellationToken = default);
        Task<ApiResponse> RequestPasswordResetAsync(string email, CancellationToken cancellationToken = default);
        Task<AuthResponse> ResetPasswordAsync(string email, string otp, string newPassword,string newPasswordVerify, CancellationToken cancellationToken = default);
        Task<AuthResponse> VerifyEmailAsync(string email, string otp, CancellationToken cancellationToken = default);
        Task<ApiResponse> ResendVerificationOtpAsync(string email, CancellationToken cancellationToken = default);
        Task<UserDetailDTO> GetUserDetailsAsync(int userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Đăng nhập bằng Google ID Token.
        /// Nếu người dùng chưa tồn tại, tự động tạo tài khoản mới.
        /// </summary>
        /// <param name="googleLoginDto">Thông tin đăng nhập Google bao gồm ID Token.</param>
        /// <param name="cancellationToken">Token hủy bỏ thao tác.</param>
        /// <returns>AuthResponse chứa access token, refresh token và thông tin người dùng.</returns>
        Task<AuthResponse> GoogleLoginAsync(GoogleLoginDto googleLoginDto, CancellationToken cancellationToken = default);
    }
}
