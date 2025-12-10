using LandingPageApp.Application.Dtos;
using LandingPageApp.Application.Interfaces;
using LandingPageApp.Domain.Repositories;
using BCrypt.Net;
using LandingPageApp.Domain.Entities;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using LandingPageApp.Application.Validations;

namespace LandingPageApp.Application.Services
{

    public class AuthService : IAuthService
    {
        private readonly ICacheRediservice _cacheRediservice;
        private readonly IEmailService _emailService;
        private readonly ISecurityService _securityService;
        private readonly ITokenService _tokenService;
        private readonly IOtpService _otpService;
        private readonly IPersonRepository _personRepository;
        private const int MinPasswordLength = 8;
        private readonly Validation _validation;



        public AuthService(
            IEmailService emailService, 
            ICacheRediservice cacheRediservice,
            ISecurityService securityService,
            IOtpService otpService,

            ITokenService tokenService,
            IPersonRepository personRepository,
            Validation validation)
        {
            _emailService = emailService;
            _cacheRediservice = cacheRediservice;
            _securityService = securityService;
            _otpService = otpService;
            _tokenService = tokenService;
            _personRepository = personRepository;
            _validation = validation;
        }


        public async Task<AuthResponse> LoginAsync(LoginDTO loginDto, CancellationToken cancellation = default)
        {
            // Validate input
            if (loginDto == null)
                throw new ArgumentNullException(nameof(loginDto));

            if (string.IsNullOrWhiteSpace(loginDto.Username))
                throw new ArgumentException("Username cannot be empty");

            if (string.IsNullOrWhiteSpace(loginDto.Password))
                throw new ArgumentException("Password cannot be empty");
            
            var person = await _personRepository.FindByUsernameAsync(loginDto.Username,cancellation);
            if (person == null)
            {
                return new AuthResponse { Status = false, Message = "Invalid credentials" };
            }

            // Verify password
            if (!_securityService.VerifyPassword(loginDto.Password, person.PasswordHash))
            {
                return new AuthResponse { Status = false, Message = "Invalid credentials" };
            }
            // Generate tokens
            var accessToken = _tokenService.GenerateAccessToken(person);
            var refreshToken = _tokenService.GenerateRefreshToken();

            // Store refresh token in Redis with 7-day expiration
            var refreshTokenKey = $"refresh_token:{person.Id}";
            await _cacheRediservice.SetAsync(refreshTokenKey, refreshToken, TimeSpan.FromDays(1));

            // Store token -> userId mapping for reverse lookup
            var tokenUserIdKey = $"refresh_token_user:{refreshToken}";
            await _cacheRediservice.SetAsync(tokenUserIdKey, person.Id.ToString(), TimeSpan.FromDays(1));

            return new AuthResponse
            {
                Status = true,
                Message = "Login successful",
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresIn = DateTime.UtcNow.AddMinutes(15),
                User = new UserDetailDTO
                {
                    Id = person.Id,
                    Username = person.Username,
                    Phone = person.Phone,
                    Email = person.Email,
                    Address = person.Address,
                    Name = person.Name,
                    LastLoginAt = DateTime.UtcNow
                }
            };
        }

        public async Task<object> RegisterAsync(RegisterDTO registerDto, CancellationToken cancellationToken = default)
        {
            try
            {
                // Validate input
                if (registerDto == null)
                    throw new ArgumentNullException(nameof(registerDto));

                if (string.IsNullOrWhiteSpace(registerDto.Name))
                    throw new ArgumentException("Name cannot be empty", nameof(registerDto.Name));

                if (string.IsNullOrWhiteSpace(registerDto.Phone))
                    throw new ArgumentException("Phone cannot be empty", nameof(registerDto.Phone));
                
                if (string.IsNullOrWhiteSpace(registerDto.Address))
                    throw new ArgumentException("Address cannot be empty", nameof(registerDto.Address));
                // Validate username is not empty
                if (string.IsNullOrWhiteSpace(registerDto.Username))
                    throw new ArgumentException("Username cannot be empty", nameof(registerDto.Username));

                // Validate email format
                if (string.IsNullOrWhiteSpace(registerDto.Email))
                    throw new ArgumentException("Email cannot be empty", nameof(registerDto.Email));

                if (!_validation.IsValidEmail(registerDto.Email))
                    throw new ArgumentException("Email format is invalid", nameof(registerDto.Email));

                // Validate password length
                if (string.IsNullOrWhiteSpace(registerDto.Password))
                    throw new ArgumentException("Password cannot be empty", nameof(registerDto.Password));

                if (registerDto.Password.Length < MinPasswordLength)
                    throw new ArgumentException($"Password must be at least {MinPasswordLength} characters long", nameof(registerDto.Password));

                // Validate password confirmation match
                if (registerDto.Password != registerDto.ConfirmPassword)
                    throw new ArgumentException("Password and confirmation password do not match");

                // Check username uniqueness
                var existingUserByUsername = await _personRepository.FindByUsernameAsync(registerDto.Username,cancellationToken);
                if (existingUserByUsername != null)
                    throw new InvalidOperationException("Username already exists");

                // Hash password using SecurityService
                var hashedPassword = _securityService.HashPassword(registerDto.Password);

                // Create Account entity
                var personal = new Person
                {
                    Username = registerDto.Username,
                    PasswordHash = hashedPassword,
                    Name = registerDto.Name,
                    Email = registerDto.Email,
                    Phone = registerDto.Phone,
                    Address = registerDto.Address,
                    RoleId = 2, // Default role ID for regular users
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                await _otpService.GenerateAndSendOtpAsync(registerDto.Email, "email-verification");

                return new  AuthResponse
                {
                    Status = true,
                    Message = "Registration successful — proceed to the next step.",

                };
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Error in RegisterAsync");
                // Trả message đẹp cho client
                return new ImformationError
                {
                    status = false,
                    error = ex.Message
                };
            }
        }

        /// <summary>
        /// Validates email format using regex pattern.
        /// </summary>
       

        public async Task<AuthResponse> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(refreshToken))
                throw new ArgumentException("Refresh token cannot be empty", nameof(refreshToken));

            // Check if refresh token is in blacklist
            var blacklistKey = $"refresh_token_blacklist:{refreshToken}";
            var isBlacklisted = await _cacheRediservice.GetAsync<bool>(blacklistKey);
            if (isBlacklisted)
                return new AuthResponse { Status = false, Message = "Invalid credentials" };

            var tokenUserIdKey = $"refresh_token_user:{refreshToken}";
            var userIdStr = await _cacheRediservice.GetAsync<string>(tokenUserIdKey);

            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out var userId))
                return new AuthResponse { Status = false, Message = "Invalid credentials" };

            // Retrieve account from repository
            var person = await _personRepository.GetByIdAsync(userId, cancellationToken);
            if (person == null)
                return new AuthResponse { Status = false, Message = "Invalid credentials" };

            // Generate new access token
            var newAccessToken = _tokenService.GenerateAccessToken(person);

            // Get user details
            //var userDto = await _personRepository.UserDetailDTO(account.Username, cancellationToken);

            return new AuthResponse
            {
                Status = true,
                Message = "Token refreshed successfully",
                AccessToken = newAccessToken,
                RefreshToken = refreshToken,
                ExpiresIn = DateTime.UtcNow.AddMinutes(15),
                User = new UserDetailDTO
                {
                    Id = person.Id,
                    Username = person.Username,
                    Email = person.Email ?? string.Empty,
                    Phone = person.Phone,
                    Address = person.Address,
                    Name = person.Name,
                    LastLoginAt = DateTime.UtcNow


                }
            };
        }

        public async Task<object> LogoutAsync(string refreshToken, CancellationToken cancellationToken = default)
        {
            try
            {
            // Validate input
                if (string.IsNullOrWhiteSpace(refreshToken))
                    throw new ArgumentException("Refresh token cannot be empty", nameof(refreshToken));

                // Add refresh token to blacklist with 7-day expiration (matching token expiration)
                var blacklistKey = $"refresh_token_blacklist:{refreshToken}";
                await _cacheRediservice.SetAsync(blacklistKey, true, TimeSpan.FromDays(1));

                // Remove the token -> userId mapping
                var tokenUserIdKey = $"refresh_token_user:{refreshToken}";
                await _cacheRediservice.RemoveAsync(tokenUserIdKey);

                return new ImformationError
                {
                    status = true,
                    error = "Logout successful"
                };
            }
            catch (Exception ex)
            {
                return new ImformationError
                {
                    status = false,
                    error = ex.Message
                };
            }
        }

        public async Task<object> RequestPasswordResetAsync(string email, CancellationToken cancellationToken = default)
        {
            try{
                // Validate input
                if (string.IsNullOrWhiteSpace(email))
                    throw new ArgumentException("Email cannot be empty", nameof(email));

                if (!_validation.IsValidEmail(email))
                    throw new ArgumentException("Email format is invalid", nameof(email));

                var person = await _personRepository.Query().FirstOrDefaultAsync(p => p.Email == email, cancellationToken);
                if (person == null)
                {
                    return new ImformationError
                    {
                        status = false,
                        error = "Account not found"
                    };
                }

                await _otpService.GenerateAndSendOtpAsync(email, "password-reset");
                return new ImformationError
                {
                    status = true,
                    error = "Password reset email sent successfully"
                };
            }
            catch (Exception ex)
            {
                return new ImformationError
                {
                    status = false,
                    error = ex.Message
                };
            }
        }

        public async Task<AuthResponse> ResetPasswordAsync(string email, string otp, string newPassword, CancellationToken cancellationToken = default)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be empty", nameof(email));
            
            if (!_validation.IsValidEmail(email))
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
                return new AuthResponse { Status = false, Message = "Invalid or expired OTP" };

            //Find account by email
            var person = await _personRepository.Query().FirstOrDefaultAsync(p => p.Email == email, cancellationToken);
            if (person == null)
                return new AuthResponse { Status = false, Message = "Account not found" };

            // Hash new password using SecurityService
            var hashedPassword = _securityService.HashPassword(newPassword);

            // Update account password
            person.PasswordHash = hashedPassword;
            person.UpdatedAt = DateTime.UtcNow;
            _personRepository.Update(person);

            //// Blacklist all existing refresh tokens for this user
            var refreshTokenKey = $"refresh_token:{person.Id}";
            var storedRefreshToken = await _cacheRediservice.GetAsync<string>(refreshTokenKey);
            if (!string.IsNullOrEmpty(storedRefreshToken))
            {
                var blacklistKey = $"refresh_token_blacklist:{storedRefreshToken}";
                await _cacheRediservice.SetAsync(blacklistKey, true, TimeSpan.FromDays(1));

                var tokenUserIdKey = $"refresh_token_user:{storedRefreshToken}";
                await _cacheRediservice.RemoveAsync(tokenUserIdKey);
            }

            await _otpService.InvalidateOtpAsync(email, "password-reset");

            return new AuthResponse
            {
                Status = true,
                Message = "Password reset successful. Please log in with your new password."
            };
        }

        public async Task<object> VerifyEmailAsync(string email, string otp, CancellationToken cancellationToken = default)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be empty", nameof(email));

            if (!_validation.IsValidEmail(email))
                throw new ArgumentException("Email format is invalid", nameof(email));

            if (string.IsNullOrWhiteSpace(otp))
                throw new ArgumentException("OTP cannot be empty", nameof(otp));

            // Validate OTP using OtpService
            var isValidOtp = await _otpService.ValidateOtpAsync(email, otp, "email-verification");
            if (!isValidOtp)
                return new AuthResponse { Status = false, Message = "Invalid or expired OTP" };

            // Find account by email
            var person = await _personRepository.Query().FirstOrDefaultAsync(p => p.Email == email, cancellationToken);

            if (person == null)
                return new AuthResponse { Status = false, Message = "Account not found" };


            _personRepository.Update(person);

            // Invalidate OTP after successful verification
            await _otpService.InvalidateOtpAsync(email, "email-verification");

            return new AuthResponse { Status = true, Message = "Email verified successfully" };
        }

        public async Task<UserDetailDTO> GetUserDetailsAsync(int userId, CancellationToken cancellationToken = default)
        {
            // Validate input
            if (userId <= 0)
                throw new ArgumentException("User ID must be greater than 0", nameof(userId));

            // Retrieve account from repository
            var person = await _personRepository.GetByIdAsync(userId, cancellationToken);
            if (person == null)
                throw new InvalidOperationException("User not found");

  

            // Map to UserDetailDTO (exclude password hash)
            var userDetailDto = new UserDetailDTO
            {
                Id = person.Id,
                Username = person.Username,
                Email = person.Email ?? string.Empty,
                Phone = person.Phone,
                Address = person.Address,
                Name = person.Name,

            };

            return userDetailDto;
        }


        public async Task<object> OtpEmailAsync(string email)
        {
            try{
            // Gửi mã OTP email
                var otp = new Random().Next(100000, 999999).ToString();
                await _emailService.SendEmailAsync(email, "OTP Code", $"Your OTP: {otp}");
                await _cacheRediservice.SetAsync(email, otp, TimeSpan.FromMinutes(5));
                return new ImformationError
                {
                    status = true,
                    error = "OTP sent successfully"
                };
            }
            catch (Exception ex)
            {
                return new ImformationError
                {
                    status = false,
                    error = ex.Message
                };
            }
        }
    }
}
