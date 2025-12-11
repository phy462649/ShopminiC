using BCrypt.Net;
using LandingPageApp.Application.Dtos;
using LandingPageApp.Application.Interfaces;
using LandingPageApp.Application.Validations;
using LandingPageApp.Domain.Entities;
using LandingPageApp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace LandingPageApp.Application.Services
{

    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _uow;
        private readonly ICacheRediservice _cacheRediservice;
        private readonly IEmailService _emailService;
        private readonly ISecurityService _securityService;
        private readonly ITokenService _tokenService;
        private readonly IOtpService _otpService;
        private readonly IPersonRepository _personRepository;
        private const int MinPasswordLength = 8;
        //private readonly EmailValidator _validation;



        public AuthService(
            IEmailService emailService,
            ICacheRediservice cacheRediservice,
            ISecurityService securityService,
            IOtpService otpService,
            IUnitOfWork unitofwork,
            ITokenService tokenService,
            IPersonRepository personRepository
)
        {
            _emailService = emailService;
            _cacheRediservice = cacheRediservice;
            _securityService = securityService;
            _otpService = otpService;
            _tokenService = tokenService;
            _personRepository = personRepository;
            _uow = unitofwork;
        }


        public async Task<AuthResponse> LoginAsync(LoginDTO loginDto, CancellationToken cancellation = default)
        {
            // Validate input
            InputValidator.NotNull(loginDto, nameof(loginDto));
            InputValidator.NotEmpty(loginDto.Username, "Username");
            InputValidator.NotNull(loginDto.Username, "Username");
            InputValidator.NotNull(loginDto.Password, "Password");
            InputValidator.NotEmpty(loginDto.Password, "Password");
            var deviceId = string.IsNullOrWhiteSpace(loginDto.DeviceToken)
                                 ? "default"
                                 : loginDto.DeviceToken;

            var person = await _personRepository.FindByUsernameAsync(loginDto.Username, cancellation);
         
            if (person == null)
            {
                return new AuthResponse { Status = false, Message = "Invalid credentials" };
            }
            if (!person.StatusVerify)
            {
                return new AuthResponse { Status = false, Message = "Please verify your email before logging in." };
            }

            // Verify password
            if (!_securityService.VerifyPassword(loginDto.Password, person.PasswordHash))
            {
                return new AuthResponse { Status = false, Message = "Invalid credentials" };
            }

            

            // Generate tokens
            var accessToken = _tokenService.GenerateAccessToken(person);
            var refreshToken = _tokenService.GenerateRefreshToken();
            //var deviceId = loginDto.DeviceToken ?? "default";

            // Store refresh token in Redis with 7-day expiration
            var refreshTokenKey = $"refresh_token:{person.Id}:{deviceId}";
            await _cacheRediservice.RemoveAsync(refreshTokenKey);
            await _cacheRediservice.SetAsync(refreshTokenKey, refreshToken, TimeSpan.FromDays(7));


            // Store token -> userId mapping for reverse lookup
            var tokenUserIdKey = $"refresh_token_user:{refreshToken}:{deviceId}";
            await _cacheRediservice.SetAsync(tokenUserIdKey, person.Id.ToString(), TimeSpan.FromDays(7));

            return new AuthResponse
            {
                Status = true,
                Message = "Login successful",
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresIn = 900,
                User = new UserDetailDTO
                {
                    Id = person.Id,
                    Username = person.Username,
                    Phone = person.Phone,
                    Email = person.Email,
                    Address = person.Address,
                    Name = person.Name,

                }
            };
        }

        public async Task<AuthResponse> RegisterAsync(RegisterDTO registerDto, CancellationToken cancellationToken = default)
        {
            await _uow.BeginTransactionAsync(cancellationToken);
            try
            {
                
                InputValidator.NotNull(registerDto, nameof(registerDto));
                InputValidator.NotEmpty(registerDto.Username, "Username");
                InputValidator.NotEmpty(registerDto.Email, "Email");
                InputValidator.NotEmpty(registerDto.Address, "Address");
                InputValidator.NotEmpty(registerDto.Password, "Password");
                InputValidator.NotEmpty(registerDto.ConfirmPassword, "ConfirmPassword");
                InputValidator.NotEmpty(registerDto.Phone, "Phone");
                InputValidator.NotEmpty(registerDto.Name, "Name");
                InputValidator.NotNull(registerDto.Username, "Username");
                InputValidator.NotNull(registerDto.Email, "Email");
                InputValidator.NotNull(registerDto.Password, "Password");
                InputValidator.NotNull(registerDto.ConfirmPassword, "ConfirmPassword");
                InputValidator.NotNull(registerDto.Phone, "Phone");
                InputValidator.NotNull(registerDto.Name, "Name");
                InputValidator.NotNull(registerDto.Address, "Address");

                EmailValidator.Validate(registerDto.Email);
                PasswordValidator.Validate(registerDto.Password);

                // Validate password confirmation match
                if (registerDto.Password != registerDto.ConfirmPassword)
                    throw new ArgumentException("Password and confirmation password do not match");

                if (await _personRepository.Query().AnyAsync(p => p.Email == registerDto.Email, cancellationToken))
                    throw new InvalidOperationException("Email already exists");

                if (await _personRepository.FindByUsernameAsync(registerDto.Username, cancellationToken) != null)
                    throw new InvalidOperationException("Username already exists");

                // Hash password using SecurityService
                var hashedPassword = _securityService.HashPassword(registerDto.Password);
                var Otp = await _otpService.GenerateOtpAsync(registerDto.Email, "email-verification");
                // Create Account entity
                var personal = new Person
                {
                    Username = registerDto.Username,
                    PasswordHash = hashedPassword,
                    Name = registerDto.Name,
                    Email = registerDto.Email,
                    Phone = registerDto.Phone,
                    Address = registerDto.Address,
                    OTP = Otp,
                    StatusVerify = false,
                    RoleId = 2, // Default role ID for regular users
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                await _personRepository.AddAsync(personal, cancellationToken);
                await _uow.SaveChangesAsync(cancellationToken);
                await _uow.CommitTransactionAsync(cancellationToken);

                try
                {
                    var subject = _emailService.GetEmailSubject("email-verification");
                    var body = _emailService.GetEmailBody(Otp, "email-verification");

                    await _emailService.SendEmailAsync(registerDto.Email, subject, body);
                }
                catch
                {
                    return new AuthResponse
                    {
                        Status = true,
                        Message = "Registration successful, but OTP email failed to send. Please retry sending OTP."
                    };
                }

                return new AuthResponse
                {
                    Status = true,
                    Message = "Registration successful — check your email for OTP."
                };
            }
            catch (Exception ex)
            {
                await _uow.RollbackTransactionAsync(cancellationToken);

                return new AuthResponse
                {
                    Status = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<AuthResponse> RefreshTokenAsync(string refreshToken,string DeviceToken, CancellationToken cancellationToken = default)
        {
            // Validate input
           InputValidator.NotEmpty(refreshToken, "RefreshToken");
            var deviceId = string.IsNullOrWhiteSpace(DeviceToken)
                                 ? "default"
                                 : DeviceToken;

            // Check if refresh token is in blacklist
            var blacklistKey = $"refresh_token_blacklist:{refreshToken}";
            var isBlacklisted = await _cacheRediservice.GetAsync<bool>(blacklistKey);
            if (isBlacklisted)
                return new AuthResponse { Status = false, Message = "Invalid credentials" };

            var tokenUserIdKey = $"refresh_token_user:{refreshToken}:{deviceId}";
            var userIdStr = await _cacheRediservice.GetAsync<string>(tokenUserIdKey);

            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out var userId))
            {
                return new AuthResponse
                {
                    Status = false,
                    Message = "Invalid credentials"
                };
            }

            // 4. Cross-check refresh token thực tế trong Redis:
            // refresh_token:{userId}:{deviceId}
            var refreshTokenKey = $"refresh_token:{userId}:{deviceId}";
            var storedRefreshToken = await _cacheRediservice.GetAsync<string>(refreshTokenKey);

            if (string.IsNullOrEmpty(storedRefreshToken) || storedRefreshToken != refreshToken)
            {
                return new AuthResponse
                {
                    Status = false,
                    Message = "Invalid credentials"
                };
            }

            // 5. Lấy user profile từ database
            var person = await _personRepository.GetByIdAsync(userId, cancellationToken);
            if (person == null)
            {
                return new AuthResponse
                {
                    Status = false,
                    Message = "Invalid credentials"
                };
            }

            // 6. Tạo access token mới
            var newAccessToken = _tokenService.GenerateAccessToken(person);

            return new AuthResponse
            {
                Status = true,
                Message = "Token refreshed successfully",
                AccessToken = newAccessToken,
                RefreshToken = refreshToken, // giữ nguyên
                ExpiresIn = 900, // 15 phút
                User = new UserDetailDTO
                {
                    Id = person.Id,
                    Username = person.Username,
                    Email = person.Email ?? string.Empty,
                    Phone = person.Phone,
                    Address = person.Address,
                    Name = person.Name
                }
            };
        }

        public async Task<ApiResponse> LogoutAsync(string refreshToken, CancellationToken cancellationToken = default)
        {
            try
            {
                // Validate input
                InputValidator.NotEmpty(refreshToken, "RefreshToken");
                InputValidator.NotNull(_cacheRediservice, nameof(_cacheRediservice));

                // Add refresh token to blacklist with 7-day expiration (matching token expiration)
                var blacklistKey = $"refresh_token_blacklist:{refreshToken}";
                await _cacheRediservice.SetAsync(blacklistKey, true, TimeSpan.FromDays(7));

                // Remove the token -> userId mapping
                var tokenUserIdKey = $"refresh_token_user:{refreshToken}";
                await _cacheRediservice.RemoveAsync(tokenUserIdKey);

                return new ApiResponse
                {
                    Status = true,
                    Message = "Logout successful"
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse
                {
                    Status = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ApiResponse> RequestPasswordResetAsync(string email, CancellationToken cancellationToken = default)
        {
            try
            {
                // Validate input
                EmailValidator.Validate(email);

                var person = await _personRepository.Query().FirstOrDefaultAsync(p => p.Email == email, cancellationToken);
                if (person == null)
                {
                    return new ApiResponse
                    {
                        Status = false,
                        Message = "Account not found"
                    };
                }

                try
                {
                    var Otp = await _otpService.GenerateOtpAsync(email, "password-reset");
                    var suject = _emailService.GetEmailSubject("password-reset");
                    var body = _emailService.GetEmailBody(Otp, "password-reset");
                    await _emailService.SendEmailAsync(email, suject, body);

                }
                catch
                {
                    return new ApiResponse
                    {
                        Status = false,
                        Message = "Password reset OTP could not be sent. Please retry."
                    };
                }
                return new ApiResponse
                {
                    Status = true,
                    Message = "Password reset OTP sent successfully"
                };

            }
            catch (Exception ex)
            {
                return new ApiResponse
                {
                    Status = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<AuthResponse> ResetPasswordAsync(string email, string otp, string newPassword,string deviceId, CancellationToken cancellationToken = default)
        {
      
            
            EmailValidator.Validate(email);
            OtpValidator.Validate(otp);
            PasswordValidator.Validate(newPassword);

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

            var deviceSetKey = $"user_devices:{person.Id}";
            var deviceIds = await _cacheRediservice.SetMembersAsync(deviceSetKey); // get all deviceIds

            foreach (var device in deviceIds)
            {
                var refreshTokenKey = $"refresh_token:{person.Id}:{deviceId}";
                var storedToken = await _cacheRediservice.GetAsync<string>(refreshTokenKey);

                if (!string.IsNullOrEmpty(storedToken))
                {
                    // Add token to blacklist
                    var blacklistKey = $"refresh_token_blacklist:{storedToken}";
                    await _cacheRediservice.SetAsync(blacklistKey, true, TimeSpan.FromDays(7));

                    // Remove token -> user mapping
                    var tokenUserIdKey = $"refresh_token_user:{storedToken}:{deviceId}";
                    await _cacheRediservice.RemoveAsync(tokenUserIdKey);

                    // Remove token key
                    await _cacheRediservice.RemoveAsync(refreshTokenKey);
                }
            }

            // Remove device list
            await _cacheRediservice.RemoveAsync(deviceSetKey);

            await _otpService.InvalidateOtpAsync(email, "password-reset");

            return new AuthResponse
            {
                Status = true,
                Message = "Password reset successful. Please log in with your new password."
            };
        }

        public async Task<AuthResponse> VerifyEmailAsync(string email, string otp, CancellationToken cancellationToken = default)
        {
            _uow.BeginTransactionAsync(cancellationToken);
            try
            {
                EmailValidator.Validate(email);

                OtpValidator.Validate(otp);

                // Validate OTP using OtpService
                var isValidOtp = await _otpService.ValidateOtpAsync(email, otp, "email-verification");
                if (!isValidOtp)
                    return new AuthResponse { Status = false, Message = "Invalid or expired OTP" };

                // Find account by email
                var person = await _personRepository.Query().FirstOrDefaultAsync(p => p.Email == email, cancellationToken);

                if (person == null)
                    return new AuthResponse { Status = false, Message = "Account not found" };

                person.StatusVerify = true;
                person.UpdatedAt = DateTime.UtcNow;
                _personRepository.Update(person);
                await _uow.SaveChangesAsync(cancellationToken);
                await _uow.CommitTransactionAsync(cancellationToken);


                // Invalidate OTP after successful verification
                await _otpService.InvalidateOtpAsync(email, "email-verification");

                return new AuthResponse { Status = true, Message = "Email verified successfully" };
            }
            catch (Exception ex)
            {
                await _uow.RollbackTransactionAsync(cancellationToken);
                return new AuthResponse { Status = false, Message = ex.Message };
            }
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


        //public async Task<ApiResponse> OtpEmailAsync(string email)
        //{
        //    try
        //    {
        //        var otp = new Random().Next(100000, 999999).ToString();
        //        await _emailService.SendEmailAsync(email, "OTP Code", $"Your OTP: {otp}");
        //        await _cacheRediservice.SetAsync(email, otp, TimeSpan.FromMinutes(5));
        //        return new ApiResponse
        //        {
        //            Status = true,
        //            Message = "OTP sent successfully"
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ApiResponse
        //        {
        //            Status = false,
        //            Message = ex.Message
        //        };
        //    }
        //}
    }
}
