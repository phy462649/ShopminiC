using LandingPageApp.Application.Dtos;
using LandingPageApp.Application.Interfaces;
using LandingPageApp.Domain.Repositories;
using BCrypt.Net;
using LandingPageApp.Domain.Entities;
using System.Text.RegularExpressions;

namespace LandingPageApp.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly ICacheRediservice _cacheRediservice;
        private readonly IEmailService _emailService;
        private readonly IAccountRepository _accountRepository;
        private readonly ISecurityService _securityService;
        private readonly IOtpService _otpService;
        private const int MinPasswordLength = 8;
        private const string EmailRegexPattern = @"^[^\s@]+@[^\s@]+\.[^\s@]+$";

        public AuthService(
            IAccountRepository accountRepository, 
            IEmailService emailService, 
            ICacheRediservice cacheRediservice,
            ISecurityService securityService,
            IOtpService otpService,
            ITokenService tokenService)
        {
            _accountRepository = accountRepository;
            _emailService = emailService;
            _cacheRediservice = cacheRediservice;
            _securityService = securityService;
            _otpService = otpService;
            _tokenService = tokenService;
        }

        private readonly ITokenService _tokenService;

        public async Task<AuthResponse> LoginAsync(LoginDTO loginDto, CancellationToken cancellation = default)
        {
            // Validate input
            if (loginDto == null)
                throw new ArgumentNullException(nameof(loginDto));

            if (string.IsNullOrWhiteSpace(loginDto.Username))
                throw new ArgumentException("Username cannot be empty", nameof(loginDto.Username));

            if (string.IsNullOrWhiteSpace(loginDto.Password))
                throw new ArgumentException("Password cannot be empty", nameof(loginDto.Password));

            // Check if account is locked
            var isLocked = await _securityService.IsAccountLockedAsync(loginDto.Username);
            if (isLocked)
                return new AuthResponse { Success = false, Message = "Invalid credentials" };

            // Find account by username
            var account = await _accountRepository.FindByUsernameAsync(loginDto.Username, cancellation);
            if (account == null)
            {
                // Record failed attempt
                await _securityService.RecordFailedLoginAttemptAsync(loginDto.Username);
                return new AuthResponse { Success = false, Message = "Invalid credentials" };
            }

            // Verify password
            if (!_securityService.VerifyPassword(loginDto.Password, account.PasswordHash))
            {
                // Record failed attempt
                await _securityService.RecordFailedLoginAttemptAsync(loginDto.Username);
                return new AuthResponse { Success = false, Message = "Invalid credentials" };
            }

            // Check account status (must be active/verified)
            if (!account.Status)
                return new AuthResponse { Success = false, Message = "Account is not verified. Please verify your email first." };

            // Generate tokens
            var accessToken = _tokenService.GenerateAccessToken(account);
            var refreshToken = _tokenService.GenerateRefreshToken();

            // Store refresh token in Redis with 7-day expiration
            var refreshTokenKey = $"refresh_token:{account.Id}";
            await _cacheRediservice.SetAsync(refreshTokenKey, refreshToken, TimeSpan.FromDays(7));

            // Store token -> userId mapping for reverse lookup
            var tokenUserIdKey = $"refresh_token_user:{refreshToken}";
            await _cacheRediservice.SetAsync(tokenUserIdKey, account.Id.ToString(), TimeSpan.FromDays(7));

            // Update last login timestamp
            account.UpdatedAt = DateTime.UtcNow;
            await _accountRepository.UpdateAsync(account, cancellation);

            // Reset failed login attempts
            await _securityService.ResetFailedLoginAttemptsAsync(loginDto.Username);

            // Get user details
            var userDto = await _accountRepository.UserDetailDTO(loginDto.Username, cancellation);

            return new AuthResponse
            {
                Success = true,
                Message = "Login successful",
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresIn = DateTime.UtcNow.AddMinutes(15),
                User = new UserDetailDTO
                {
                    Id = userDto.Id,
                    Username = userDto.Username,
                    Email = userDto.Email ?? string.Empty,
                    Status = userDto.Status,
                    LastLoginAt = DateTime.UtcNow
                }
            };
        }

        public async Task<AuthResponse> RegisterAsync(RegisterDTO registerDto, CancellationToken cancellationToken = default)
        {
            // Validate input
            if (registerDto == null)
                throw new ArgumentNullException(nameof(registerDto));

            // Validate username is not empty
            if (string.IsNullOrWhiteSpace(registerDto.Username))
                throw new ArgumentException("Username cannot be empty", nameof(registerDto.Username));

            // Validate email format
            if (string.IsNullOrWhiteSpace(registerDto.Email))
                throw new ArgumentException("Email cannot be empty", nameof(registerDto.Email));

            if (!IsValidEmail(registerDto.Email))
                throw new ArgumentException("Email format is invalid", nameof(registerDto.Email));

            // Validate password length
            if (string.IsNullOrWhiteSpace(registerDto.Password))
                throw new ArgumentException("Password cannot be empty", nameof(registerDto.Password));

            if (registerDto.Password.Length < MinPasswordLength)
                throw new ArgumentException($"Password must be at least {MinPasswordLength} characters long", nameof(registerDto.Password));

            // Validate password confirmation match
            if (registerDto.Password != registerDto.ConfirmPassword)
                throw new ArgumentException("Password and confirmation password do not match", nameof(registerDto.ConfirmPassword));

            // Check username uniqueness
            var existingUserByUsername = await _accountRepository.GetByUsernameAsync(registerDto.Username);
            if (existingUserByUsername != null)
                throw new InvalidOperationException("Username already exists");

            // Hash password using SecurityService
            var hashedPassword = _securityService.HashPassword(registerDto.Password);

            // Create Account entity
            var account = new Account
            {
                Username = registerDto.Username,
                PasswordHash = hashedPassword,
                Status = false, // Account starts as unverified
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Add account to repository
            await _accountRepository.AddAsync(account, cancellationToken);

            // Generate and send verification OTP
            await _otpService.GenerateAndSendOtpAsync(registerDto.Email, "email-verification");

            // Get user details
            var userDto = await _accountRepository.UserDetailDTO(account.Username, cancellationToken);

            return new AuthResponse
            {
                Success = true,
                Message = "Registration successful. Please verify your email with the OTP sent to your email address.",
                User = new UserDetailDTO
                {
                    Id = userDto.Id,
                    Username = userDto.Username,
                    Email = registerDto.Email
                }
            };
        }

        /// <summary>
        /// Validates email format using regex pattern.
        /// </summary>
        private bool IsValidEmail(string email)
        {
            try
            {
                return Regex.IsMatch(email, EmailRegexPattern, RegexOptions.IgnoreCase);
            }
            catch
            {
                return false;
            }
        }

        public async Task<AuthResponse> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(refreshToken))
                throw new ArgumentException("Refresh token cannot be empty", nameof(refreshToken));

            // Check if refresh token is in blacklist
            var blacklistKey = $"refresh_token_blacklist:{refreshToken}";
            var isBlacklisted = await _cacheRediservice.GetAsync<bool>(blacklistKey);
            if (isBlacklisted)
                return new AuthResponse { Success = false, Message = "Invalid credentials" };

            // Try to extract user ID from the refresh token stored in Redis
            // We need to search for the token in Redis to find the associated user ID
            // Since refresh tokens are stored as refresh_token:{userId}, we need a reverse lookup
            // For now, we'll use a different approach: store token -> userId mapping
            var tokenUserIdKey = $"refresh_token_user:{refreshToken}";
            var userIdStr = await _cacheRediservice.GetAsync<string>(tokenUserIdKey);

            if (string.IsNullOrEmpty(userIdStr) || !long.TryParse(userIdStr, out var userId))
                return new AuthResponse { Success = false, Message = "Invalid credentials" };

            // Retrieve account from repository
            var account = await _accountRepository.GetByIdAsync(userId, cancellationToken);
            if (account == null)
                return new AuthResponse { Success = false, Message = "Invalid credentials" };

            // Generate new access token
            var newAccessToken = _tokenService.GenerateAccessToken(account);

            // Get user details
            var userDto = await _accountRepository.UserDetailDTO(account.Username, cancellationToken);

            return new AuthResponse
            {
                Success = true,
                Message = "Token refreshed successfully",
                AccessToken = newAccessToken,
                RefreshToken = refreshToken,
                ExpiresIn = DateTime.UtcNow.AddMinutes(15),
                User = new UserDetailDTO
                {
                    Id = userDto.Id,
                    Username = userDto.Username,
                    Email = userDto.Email ?? string.Empty,
                    Status = userDto.Status
                }
            };
        }

        public async Task<bool> LogoutAsync(string refreshToken, CancellationToken cancellationToken = default)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(refreshToken))
                throw new ArgumentException("Refresh token cannot be empty", nameof(refreshToken));

            // Add refresh token to blacklist with 7-day expiration (matching token expiration)
            var blacklistKey = $"refresh_token_blacklist:{refreshToken}";
            await _cacheRediservice.SetAsync(blacklistKey, true, TimeSpan.FromDays(7));

            // Remove the token -> userId mapping
            var tokenUserIdKey = $"refresh_token_user:{refreshToken}";
            await _cacheRediservice.RemoveAsync(tokenUserIdKey);

            return true;
        }

        public async Task<bool> RequestPasswordResetAsync(string email, CancellationToken cancellationToken = default)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be empty", nameof(email));

            if (!IsValidEmail(email))
                throw new ArgumentException("Email format is invalid", nameof(email));

            // Find account by email
            var account = await _accountRepository.FindByEmailAsync(email, cancellationToken);
            if (account == null)
            {
                // Return success even if account not found (security best practice - don't reveal if email exists)
                return true;
            }

            // Generate and send OTP via OtpService
            await _otpService.GenerateAndSendOtpAsync(email, "password-reset");

            return true;
        }

        public async Task<AuthResponse> ResetPasswordAsync(string email, string otp, string newPassword, CancellationToken cancellationToken = default)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be empty", nameof(email));

            if (!IsValidEmail(email))
                throw new ArgumentException("Email format is invalid", nameof(email));

            if (string.IsNullOrWhiteSpace(otp))
                throw new ArgumentException("OTP cannot be empty", nameof(otp));

            if (string.IsNullOrWhiteSpace(newPassword))
                throw new ArgumentException("New password cannot be empty", nameof(newPassword));

            // Validate new password length
            if (newPassword.Length < MinPasswordLength)
                throw new ArgumentException($"Password must be at least {MinPasswordLength} characters long", nameof(newPassword));

            // Validate OTP
            var isValidOtp = await _otpService.ValidateOtpAsync(email, otp, "password-reset");
            if (!isValidOtp)
                return new AuthResponse { Success = false, Message = "Invalid or expired OTP" };

            // Find account by email
            var account = await _accountRepository.FindByEmailAsync(email, cancellationToken);
            if (account == null)
                return new AuthResponse { Success = false, Message = "Account not found" };

            // Hash new password using SecurityService
            var hashedPassword = _securityService.HashPassword(newPassword);

            // Update account password
            account.PasswordHash = hashedPassword;
            account.UpdatedAt = DateTime.UtcNow;
            await _accountRepository.UpdateAsync(account, cancellationToken);

            // Blacklist all existing refresh tokens for this user
            var refreshTokenKey = $"refresh_token:{account.Id}";
            var storedRefreshToken = await _cacheRediservice.GetAsync<string>(refreshTokenKey);
            if (!string.IsNullOrEmpty(storedRefreshToken))
            {
                var blacklistKey = $"refresh_token_blacklist:{storedRefreshToken}";
                await _cacheRediservice.SetAsync(blacklistKey, true, TimeSpan.FromDays(7));
                
                var tokenUserIdKey = $"refresh_token_user:{storedRefreshToken}";
                await _cacheRediservice.RemoveAsync(tokenUserIdKey);
            }

            // Invalidate OTP after successful reset
            await _otpService.InvalidateOtpAsync(email, "password-reset");

            return new AuthResponse
            {
                Success = true,
                Message = "Password reset successful. Please log in with your new password."
            };
        }

        public async Task<bool> VerifyEmailAsync(string email, string otp, CancellationToken cancellationToken = default)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be empty", nameof(email));

            if (!IsValidEmail(email))
                throw new ArgumentException("Email format is invalid", nameof(email));

            if (string.IsNullOrWhiteSpace(otp))
                throw new ArgumentException("OTP cannot be empty", nameof(otp));

            // Validate OTP using OtpService
            var isValidOtp = await _otpService.ValidateOtpAsync(email, otp, "email-verification");
            if (!isValidOtp)
                return false;

            // Find account by email
            var account = await _accountRepository.FindByEmailAsync(email, cancellationToken);
            if (account == null)
                return false;

            // Mark account as verified (set Status = true)
            account.Status = true;
            account.UpdatedAt = DateTime.UtcNow;
            await _accountRepository.UpdateAsync(account, cancellationToken);

            // Invalidate OTP after successful verification
            await _otpService.InvalidateOtpAsync(email, "email-verification");

            return true;
        }

        public async Task<UserDetailDTO> GetUserDetailsAsync(long userId, CancellationToken cancellationToken = default)
        {
            // Validate input
            if (userId <= 0)
                throw new ArgumentException("User ID must be greater than 0", nameof(userId));

            // Retrieve account from repository
            var account = await _accountRepository.GetByIdAsync(userId, cancellationToken);
            if (account == null)
                throw new InvalidOperationException("User not found");

            // Get detailed user information
            var userDetail = await _accountRepository.UserDetailDTO(account.Username, cancellationToken);
            if (userDetail == null)
                throw new InvalidOperationException("User details not found");

            // Map to UserDetailDTO (exclude password hash)
            var userDetailDto = new UserDetailDTO
            {
                Id = userDetail.Id,
                Username = userDetail.Username,
                Email = userDetail.Email ?? string.Empty,
                Status = userDetail.Status,
                CreatedAt = userDetail.CreatedAt,
                LastLoginAt = userDetail.UpdatedAt
            };

            return userDetailDto;
        }

        public async Task LogoutAsync()
        {
            // Có thể xóa token hoặc refresh token khỏi Redis
            await Task.CompletedTask;
        }

        public async Task<bool> OtpEmailAsync(string email)
        {
            // Gửi mã OTP email
            var otp = new Random().Next(100000, 999999).ToString();
            await _emailService.SendMailAsync(email, "OTP Code", $"Your OTP: {otp}");
            await _cacheRediservice.SetAsync(email, otp, TimeSpan.FromMinutes(5));
            return true;
        }
    }
}
