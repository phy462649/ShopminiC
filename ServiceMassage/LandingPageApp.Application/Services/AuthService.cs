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
    /// <summary>
    /// Service xử lý các chức năng xác thực người dùng.
    /// Bao gồm: đăng nhập, đăng ký, quên mật khẩu, đặt lại mật khẩu, xác thực email và quản lý token.
    /// </summary>
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


        /// <summary>
        /// Khởi tạo một instance mới của <see cref="AuthService"/>.
        /// </summary>
        /// <param name="emailService">Service gửi email.</param>
        /// <param name="cacheRediservice">Service cache Redis.</param>
        /// <param name="securityService">Service bảo mật (hash/verify password).</param>
        /// <param name="otpService">Service quản lý OTP.</param>
        /// <param name="unitofwork">Unit of Work để quản lý transaction.</param>
        /// <param name="tokenService">Service tạo và quản lý token.</param>
        /// <param name="personRepository">Repository quản lý dữ liệu người dùng.</param>
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

        /// <summary>
        /// Xử lý đăng nhập người dùng.
        /// Xác thực thông tin đăng nhập, kiểm tra trạng thái xác thực email,
        /// tạo access token và refresh token nếu thành công.
        /// </summary>
        /// <param name="loginDto">Thông tin đăng nhập bao gồm username, password và device token.</param>
        /// <param name="cancellation">Token để hủy thao tác bất đồng bộ.</param>
        /// <returns>
        /// <see cref="AuthResponse"/> chứa:
        /// - Status: true nếu đăng nhập thành công, false nếu thất bại.
        /// - Message: Thông báo kết quả.
        /// - AccessToken: JWT access token (nếu thành công).
        /// - RefreshToken: Refresh token để làm mới access token (nếu thành công).
        /// - ExpiresIn: Thời gian hết hạn của access token (giây).
        /// - User: Thông tin chi tiết người dùng (nếu thành công).
        /// </returns>
        public async Task<AuthResponse> LoginAsync(LoginDTO loginDto, CancellationToken cancellation = default)
        {
            try
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
                //person.Role = _personRepository.GetRoleNameById(person.RoleId,cancellation);
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
                        Phone = person.Phone ?? string.Empty,
                        Email = person.Email ?? string.Empty,
                        Address = person.Address ?? string.Empty,
                        Name = person.Name,

                    }
                };
            }
            catch (Exception ex)
            {
                return new AuthResponse
                {
                    Status = false,
                    Message = $"Login failed: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Xử lý đăng ký tài khoản người dùng mới.
        /// Tạo tài khoản với thông tin được cung cấp, gửi OTP xác thực email.
        /// </summary>
        /// <param name="registerDto">Thông tin đăng ký bao gồm username, email, password, phone, name, address.</param>
        /// <param name="cancellationToken">Token để hủy thao tác bất đồng bộ.</param>
        /// <returns>
        /// <see cref="AuthResponse"/> chứa:
        /// - Status: true nếu đăng ký thành công, false nếu thất bại.
        /// - Message: Thông báo kết quả (yêu cầu kiểm tra email để lấy OTP nếu thành công).
        /// </returns>
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

        /// <summary>
        /// Làm mới access token bằng refresh token.
        /// Kiểm tra tính hợp lệ của refresh token và tạo access token mới.
        /// </summary>
        /// <param name="refreshToken">Refresh token hiện tại của người dùng.</param>
        /// <param name="DeviceToken">Mã định danh thiết bị của người dùng.</param>
        /// <param name="cancellationToken">Token để hủy thao tác bất đồng bộ.</param>
        /// <returns>
        /// <see cref="AuthResponse"/> chứa:
        /// - Status: true nếu làm mới thành công, false nếu thất bại.
        /// - Message: Thông báo kết quả.
        /// - AccessToken: JWT access token mới (nếu thành công).
        /// - RefreshToken: Refresh token (giữ nguyên).
        /// - ExpiresIn: Thời gian hết hạn của access token mới (giây).
        /// - User: Thông tin chi tiết người dùng (nếu thành công).
        /// </returns>
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
                    Phone = person.Phone ?? string.Empty,
                    Address = person.Address ?? string.Empty,
                    Name = person.Name
                }
            };
        }

        /// <summary>
        /// Xử lý đăng xuất người dùng.
        /// Thêm refresh token vào blacklist và xóa mapping token-user trong cache.
        /// </summary>
        /// <param name="refreshToken">Refresh token cần vô hiệu hóa.</param>
        /// <param name="cancellationToken">Token để hủy thao tác bất đồng bộ.</param>
        /// <returns>
        /// <see cref="ApiResponse"/> chứa:
        /// - Status: true nếu đăng xuất thành công, false nếu thất bại.
        /// - Message: Thông báo kết quả.
        /// </returns>
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

        /// <summary>
        /// Yêu cầu đặt lại mật khẩu.
        /// Gửi OTP đến email người dùng để xác thực yêu cầu đặt lại mật khẩu.
        /// </summary>
        /// <param name="email">Địa chỉ email của tài khoản cần đặt lại mật khẩu.</param>
        /// <param name="cancellationToken">Token để hủy thao tác bất đồng bộ.</param>
        /// <returns>
        /// <see cref="ApiResponse"/> chứa:
        /// - Status: true nếu gửi OTP thành công, false nếu thất bại.
        /// - Message: Thông báo kết quả.
        /// </returns>
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

        /// <summary>
        /// Đặt lại mật khẩu người dùng.
        /// Xác thực OTP và cập nhật mật khẩu mới, đồng thời vô hiệu hóa tất cả refresh token của người dùng.
        /// </summary>
        /// <param name="email">Địa chỉ email của tài khoản.</param>
        /// <param name="otp">Mã OTP xác thực.</param>
        /// <param name="newPassword">Mật khẩu mới.</param>
        /// <param name="newPasswordVerify">Xác nhận mật khẩu mới.</param>
        /// <param name="cancellationToken">Token để hủy thao tác bất đồng bộ.</param>
        /// <returns>
        /// <see cref="AuthResponse"/> chứa:
        /// - Status: true nếu đặt lại mật khẩu thành công, false nếu thất bại.
        /// - Message: Thông báo kết quả.
        /// </returns>
        public async Task<AuthResponse> ResetPasswordAsync(string email, string otp, string newPassword,string newPasswordVerify, CancellationToken cancellationToken = default)
        {
      
            
            EmailValidator.Validate(email);
            OtpValidator.Validate(otp);
            PasswordValidator.Validate(newPassword);
            PasswordValidator.Validate(newPasswordVerify);

            if(!newPassword.Equals(newPasswordVerify))
            {
                return new AuthResponse { Status = false, Message = "Password and PasswordConfirm don't match" };
            }    
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

            // Invalidate all refresh tokens for this user (logout from all devices)
            var deviceSetKey = $"user_devices:{person.Id}";
            var deviceIds = await _cacheRediservice.SetMembersAsync(deviceSetKey);

            foreach (var deviceId in deviceIds)
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

            await _uow.SaveChangesAsync(cancellationToken);
            await _otpService.InvalidateOtpAsync(email, "password-reset");

            return new AuthResponse
            {
                Status = true,
                Message = "Password reset successful. Please log in with your new password."
            };
        }

        /// <summary>
        /// Xác thực địa chỉ email của người dùng.
        /// Kiểm tra OTP và cập nhật trạng thái xác thực email của tài khoản.
        /// </summary>
        /// <param name="email">Địa chỉ email cần xác thực.</param>
        /// <param name="otp">Mã OTP xác thực.</param>
        /// <param name="cancellationToken">Token để hủy thao tác bất đồng bộ.</param>
        /// <returns>
        /// <see cref="AuthResponse"/> chứa:
        /// - Status: true nếu xác thực email thành công, false nếu thất bại.
        /// - Message: Thông báo kết quả.
        /// </returns>
        public async Task<AuthResponse> VerifyEmailAsync(string email, string otp, CancellationToken cancellationToken = default)
        {
            await  _uow.BeginTransactionAsync(cancellationToken);
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

        /// <summary>
        /// Gửi lại OTP xác thực email.
        /// Tạo OTP mới và gửi đến email của người dùng chưa xác thực.
        /// </summary>
        /// <param name="email">Địa chỉ email cần gửi lại OTP.</param>
        /// <param name="cancellationToken">Token để hủy thao tác bất đồng bộ.</param>
        /// <returns>
        /// <see cref="ApiResponse"/> chứa:
        /// - Status: true nếu gửi OTP thành công, false nếu thất bại.
        /// - Message: Thông báo kết quả.
        /// </returns>
        public async Task<ApiResponse> ResendVerificationOtpAsync(string email, CancellationToken cancellationToken = default)
        {
            try
            {
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

                if (person.StatusVerify)
                {
                    return new ApiResponse
                    {
                        Status = false,
                        Message = "Email already verified"
                    };
                }

                try
                {
                    var otp = await _otpService.GenerateOtpAsync(email, "email-verification");
                    var subject = _emailService.GetEmailSubject("email-verification");
                    var body = _emailService.GetEmailBody(otp, "email-verification");
                    await _emailService.SendEmailAsync(email, subject, body);
                }
                catch
                {
                    return new ApiResponse
                    {
                        Status = false,
                        Message = "Verification OTP could not be sent. Please retry."
                    };
                }

                return new ApiResponse
                {
                    Status = true,
                    Message = "Verification OTP sent successfully"
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

        /// <summary>
        /// Lấy thông tin chi tiết của người dùng theo ID.
        /// </summary>
        /// <param name="userId">ID của người dùng cần lấy thông tin.</param>
        /// <param name="cancellationToken">Token để hủy thao tác bất đồng bộ.</param>
        /// <returns>
        /// <see cref="UserDetailDTO"/> chứa thông tin chi tiết người dùng bao gồm:
        /// Id, Username, Email, Phone, Address, Name.
        /// </returns>
        /// <exception cref="ArgumentException">Ném ra khi userId không hợp lệ (nhỏ hơn hoặc bằng 0).</exception>
        /// <exception cref="InvalidOperationException">Ném ra khi không tìm thấy người dùng.</exception>
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
                Phone = person.Phone ?? string.Empty,
                Address = person.Address ?? string.Empty,
                Name = person.Name,

            };

            return userDetailDto;
        }

        /// <summary>
        /// Đăng nhập bằng Google ID Token.
        /// Xác thực token với Google, nếu người dùng chưa tồn tại sẽ tự động tạo tài khoản mới.
        /// </summary>
        /// <param name="googleLoginDto">Thông tin đăng nhập Google bao gồm ID Token.</param>
        /// <param name="cancellationToken">Token hủy bỏ thao tác.</param>
        /// <returns>AuthResponse chứa access token, refresh token và thông tin người dùng.</returns>
        public async Task<AuthResponse> GoogleLoginAsync(GoogleLoginDto googleLoginDto, CancellationToken cancellationToken = default)
        {
            try
            {
                // Validate input
                InputValidator.NotNull(googleLoginDto, nameof(googleLoginDto));
                InputValidator.NotEmpty(googleLoginDto.IdToken, "IdToken");

                var deviceId = string.IsNullOrWhiteSpace(googleLoginDto.DeviceToken)
                    ? "default"
                    : googleLoginDto.DeviceToken;

                // Xác thực Google ID Token
                var payload = await Google.Apis.Auth.GoogleJsonWebSignature.ValidateAsync(googleLoginDto.IdToken);

                if (payload == null)
                {
                    return new AuthResponse { Status = false, Message = "Invalid Google token" };
                }

                // Lấy thông tin từ payload
                var googleId = payload.Subject;
                var email = payload.Email;
                var name = payload.Name ?? email.Split('@')[0];
                var emailVerified = payload.EmailVerified;

                if (string.IsNullOrEmpty(email))
                {
                    return new AuthResponse { Status = false, Message = "Email not found in Google token" };
                }

                // Tìm người dùng theo email
                var person = await _personRepository.Query()
                    .FirstOrDefaultAsync(p => p.Email == email, cancellationToken);

                if (person == null)
                {
                    // Tạo tài khoản mới cho người dùng Google
                    await _uow.BeginTransactionAsync(cancellationToken);
                    try
                    {
                        // Tạo username từ email (loại bỏ phần @domain)
                        var baseUsername = email.Split('@')[0];
                        var username = baseUsername;
                        var counter = 1;

                        // Kiểm tra username đã tồn tại chưa, nếu có thì thêm số
                        while (await _personRepository.FindByUsernameAsync(username, cancellationToken) != null)
                        {
                            username = $"{baseUsername}{counter}";
                            counter++;
                        }

                        // Tạo password ngẫu nhiên (người dùng Google không cần dùng)
                        var randomPassword = Guid.NewGuid().ToString("N")[..16];
                        var hashedPassword = _securityService.HashPassword(randomPassword);

                        person = new Person
                        {
                            Username = username,
                            PasswordHash = hashedPassword,
                            Name = name,
                            Email = email,
                            Phone = string.Empty,
                            Address = string.Empty,
                            GoogleId = googleId,
                            StatusVerify = emailVerified, // Email từ Google đã được xác thực
                            RoleId = 2, // Default role ID for regular users
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        };

                        await _personRepository.AddAsync(person, cancellationToken);
                        await _uow.SaveChangesAsync(cancellationToken);
                        await _uow.CommitTransactionAsync(cancellationToken);
                    }
                    catch
                    {
                        await _uow.RollbackTransactionAsync(cancellationToken);
                        throw;
                    }
                }
                else
                {
                    // Cập nhật GoogleId nếu chưa có
                    if (string.IsNullOrEmpty(person.GoogleId))
                    {
                        person.GoogleId = googleId;
                        person.UpdatedAt = DateTime.UtcNow;
                        _personRepository.Update(person);
                        await _uow.SaveChangesAsync(cancellationToken);
                    }

                    // Nếu email chưa được xác thực, cập nhật trạng thái
                    if (!person.StatusVerify && emailVerified)
                    {
                        person.StatusVerify = true;
                        person.UpdatedAt = DateTime.UtcNow;
                        _personRepository.Update(person);
                        await _uow.SaveChangesAsync(cancellationToken);
                    }
                }

                // Tạo tokens
                var accessToken = _tokenService.GenerateAccessToken(person);
                var refreshToken = _tokenService.GenerateRefreshToken();

                // Lưu refresh token vào Redis
                var refreshTokenKey = $"refresh_token:{person.Id}:{deviceId}";
                await _cacheRediservice.RemoveAsync(refreshTokenKey);
                await _cacheRediservice.SetAsync(refreshTokenKey, refreshToken, TimeSpan.FromDays(7));

                // Lưu mapping token -> userId
                var tokenUserIdKey = $"refresh_token_user:{refreshToken}:{deviceId}";
                await _cacheRediservice.SetAsync(tokenUserIdKey, person.Id.ToString(), TimeSpan.FromDays(7));

                return new AuthResponse
                {
                    Status = true,
                    Message = "Google login successful",
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    ExpiresIn = 900,
                    User = new UserDetailDTO
                    {
                        Id = person.Id,
                        Username = person.Username,
                        Phone = person.Phone ?? string.Empty,
                        Email = person.Email ?? string.Empty,
                        Address = person.Address ?? string.Empty,
                        Name = person.Name,
                    }
                };
            }
            catch (Google.Apis.Auth.InvalidJwtException)
            {
                return new AuthResponse
                {
                    Status = false,
                    Message = "Invalid or expired Google token"
                };
            }
            catch (Exception ex)
            {
                return new AuthResponse
                {
                    Status = false,
                    Message = $"Google login failed: {ex.Message}"
                };
            }
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
