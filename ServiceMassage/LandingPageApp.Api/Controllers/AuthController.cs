using LandingPageApp.Application.Dtos;
using LandingPageApp.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LandingPageApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        /// <summary>
        /// Login with username and password.
        /// </summary>
        /// <param name="loginDto">Login credentials</param>
        /// <returns>AuthResponse with access token, refresh token, and user details</returns>
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
        /// Register a new user account.
        /// </summary>
        /// <param name="registerDto">Registration details including username, email, password</param>
        /// <returns>AuthResponse with user details and success message</returns>
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
        /// <param name="refreshToken">The refresh token</param>
        /// <returns>AuthResponse with new access token</returns>
        [HttpPost("refresh")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AuthResponse>> RefreshToken([FromBody] string refreshToken,string device)
        {
            var result = await _authService.RefreshTokenAsync(refreshToken,device);
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
        /// <param name="email">Email address for password reset</param>
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
        /// <param name="resetRequest">Email, OTP, and new password</param>
        /// <returns>AuthResponse with success message</returns>
        [HttpPost("reset-password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<AuthResponse>> ResetPassword([FromBody] PasswordResetRequest resetRequest,string device)
        {
            var result = await _authService.ResetPasswordAsync(resetRequest.Email, resetRequest.Otp, resetRequest.NewPassword,device);
            return Ok(result);
        }
        /// <summary>
        /// Verify email address using OTP.
        /// </summary>
        /// <param name="verifyRequest">Email and OTP for verification</param>
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
        /// Get current user details.
        /// </summary>
        /// <returns>UserDetailDTO with user information</returns>
   
    }
}
