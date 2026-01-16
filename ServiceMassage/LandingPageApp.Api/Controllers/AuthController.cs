using LandingPageApp.Application.Dtos;
using LandingPageApp.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;

namespace LandingPageApp.Api.Controllers
{
    /// <summary>
    /// Controller xử lý các chức năng xác thực người dùng.
    /// Bao gồm: đăng nhập, đăng ký, đăng nhập Google, quên mật khẩu, xác thực email.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [EnableRateLimiting("auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Đăng nhập bằng username và password.
        /// </summary>
        /// <param name="loginDto">Thông tin đăng nhập</param>
        /// <returns>AuthResponse chứa access token, refresh token và thông tin người dùng</returns>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginDTO loginDto)
        {
            var result = await _authService.LoginAsync(loginDto);
            return Ok(result);
        }

        /// <summary>
        /// Đăng nhập bằng Google.
        /// Nếu người dùng chưa có tài khoản, hệ thống sẽ tự động tạo tài khoản mới.
        /// </summary>
        /// <param name="googleLoginDto">Thông tin đăng nhập Google bao gồm ID Token</param>
        /// <returns>AuthResponse chứa access token, refresh token và thông tin người dùng</returns>
        [HttpPost("google-login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AuthResponse>> GoogleLogin([FromBody] GoogleLoginDto googleLoginDto)
        {
            var result = await _authService.GoogleLoginAsync(googleLoginDto);
            if (!result.Status)
            {
                return Unauthorized(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Đăng ký tài khoản mới.
        /// </summary>
        /// <param name="registerDto">Thông tin đăng ký bao gồm username, email, password</param>
        /// <returns>AuthResponse với thông báo kết quả</returns>
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterDTO registerDto)
        {
            var result = await _authService.RegisterAsync(registerDto);
            return Ok(result);
        }
        /// <summary>
        /// Refresh access token using refresh token.
        /// </summary>
        /// <param name="request">The refresh token request containing refreshToken and deviceToken</param>
        /// <returns>AuthResponse with new access token</returns>
        [HttpPost("refresh")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AuthResponse>> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            var result = await _authService.RefreshTokenAsync(request.RefreshToken, request.DeviceToken);
            return Ok(result);
        }
        /// <summary>
        /// Logout and invalidate refresh token.
        /// </summary>
        /// <param name="refreshToken">The refresh token to invalidate</param>
        /// <returns>Success response</returns>
        [HttpPost("logout")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<object>> Logout([FromBody] string refreshToken)
        {
            var success = await _authService.LogoutAsync(refreshToken);
            return Ok(new { success, message = "Logout successful" });
        }
        /// <summary>
        /// Request password reset by sending OTP to email.
        /// </summary>
        /// <param name="email">gửi mail quên mật khẩu</param>
        /// <returns>Success response</returns>
        [HttpPost("request-password-reset")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<object>> RequestPasswordReset([FromBody] string email)
        {
            var success = await _authService.RequestPasswordResetAsync(email);
            return Ok(new { success, message = "If the email exists, a password reset code has been sent" });
        }

        /// <summary>
        /// Reset password using OTP and new password.
        /// </summary>
        /// <param name="resetRequest">quên mật khẩu</param>
        /// <returns>AuthResponse with success message</returns>
        [HttpPost("reset-password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<AuthResponse>> ResetPassword([FromBody] PasswordResetRequest resetRequest)
        {
            var result = await _authService.ResetPasswordAsync(resetRequest.Email, resetRequest.Otp, resetRequest.NewPassword,resetRequest.NewPasswordVerify);
            return Ok(result);
        }
        /// <summary>
        /// Verify email address using OTP.
        /// </summary>
        /// <param name="verifyRequest"> xác nhận đăng kí tài khoản </param>
        /// <returns>Success response</returns>
        [HttpPost("verify-email")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<object>> VerifyEmail([FromBody] EmailVerificationRequest verifyRequest)
        {
            var success = await _authService.VerifyEmailAsync(verifyRequest.Email, verifyRequest.Otp);
            if (!((AuthResponse)success).Status)
                return BadRequest(success);

            return Ok(success);
        }

        /// <summary>
        /// Kiểm tra user hiện tại có phải Admin không.
        /// </summary>
        /// <returns>200 nếu là Admin, 403 nếu không phải</returns>
        [HttpGet("isAdmin")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<object> IsAdmin()
        {
            // Thử nhiều cách lấy role claim
            var roleClaim = User.FindFirst("role")?.Value 
                ?? User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            
            var isAdmin = roleClaim?.Equals("ADMIN", StringComparison.OrdinalIgnoreCase) ?? false;
            
            if (!isAdmin)
                return StatusCode(403, new { status = false });
            
            return Ok(new { status = true });
        }

    }
}
