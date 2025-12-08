using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LandingPageApp.Application.Dtos;

namespace LandingPageApp.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponse> LoginAsync(LoginDTO loginDto, CancellationToken cancellationToken = default);
        Task<AuthResponse> RegisterAsync(RegisterDTO registerDto, CancellationToken cancellationToken = default);
        Task<AuthResponse> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
        Task<bool> LogoutAsync(string refreshToken, CancellationToken cancellationToken = default);
        Task<bool> RequestPasswordResetAsync(string email, CancellationToken cancellationToken = default);
        Task<AuthResponse> ResetPasswordAsync(string email, string otp, string newPassword, CancellationToken cancellationToken = default);
        Task<bool> VerifyEmailAsync(string email, string otp, CancellationToken cancellationToken = default);
        Task<UserDetailDTO> GetUserDetailsAsync(long userId, CancellationToken cancellationToken = default);
    }
}
