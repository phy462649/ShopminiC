using System;
using System.Threading.Tasks;
using LandingPageApp.Application.Interfaces;

namespace LandingPageApp.Application.Services;

/// <summary>
/// OtpService handles OTP generation, validation, and management.
/// Implements IOtpService interface for OTP operations.
/// </summary>
public class OtpService : IOtpService
{
    private readonly ICacheRediservice _cacheService;
    private readonly IEmailService _emailService;
    private const int OtpLength = 6;
    private const int MaxFailedAttempts = 3;
    private const int FailedAttemptLockoutMinutes = 15;
    private const int VerificationOtpExpirationHours = 24;
    private const int PasswordResetOtpExpirationMinutes = 15;
    private const string OtpKeyPrefix = "otp:";
    private const string FailedAttemptsKeyPrefix = "otp_failed_attempts:";
    private const string OtpLockedKeyPrefix = "otp_locked:";

    public OtpService(ICacheRediservice cacheService, IEmailService emailService)
    {
        _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
    }

    /// <summary>
    /// Generates a random 6-digit OTP, sends it via email, and stores it in Redis.
    /// </summary>
    /// <param name="email">The email address to send OTP to</param>
    /// <param name="purpose">The purpose of OTP (e.g., "registration", "password-reset")</param>
    /// <returns>The generated OTP code</returns>
    public async Task<string> GenerateOtpAsync(string email, string purpose)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be null or empty", nameof(email));

        if (string.IsNullOrWhiteSpace(purpose))
            throw new ArgumentException("Purpose cannot be null or empty", nameof(purpose));

        // Generate 6-digit OTP
        var otp = GenerateOtp();
        var normalizedEmail = email.ToLower();
        var otpKey = $"{OtpKeyPrefix}{normalizedEmail}:{purpose}";

        // Determine expiration based on purpose
        var expiration = TimeSpan.FromMinutes(15);


        // Invalidate previous OTP if exists
        await _cacheService.RemoveAsync(otpKey);

        // Store OTP in Redis
        await _cacheService.SetAsync(otpKey, otp, expiration);

        // Send OTP via email
        //var subject = GetEmailSubject(purpose);
        //var body = GetEmailBody(otp, purpose);
        //await _emailService.SendEmailAsync(email, subject, body);

        return otp;
    }

    /// <summary>
    /// Validates an OTP code against the stored value.
    /// Implements rate limiting with 3 failed attempts per 15 minutes.
    /// </summary>
    /// <param name="email">The email address associated with the OTP</param>
    /// <param name="otp">The OTP code to validate</param>
    /// <param name="purpose">The purpose of OTP validation</param>
    /// <returns>True if OTP is valid, false otherwise</returns>
    public async Task<bool> ValidateOtpAsync(string email, string otp, string purpose)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be null or empty", nameof(email));

        if (string.IsNullOrWhiteSpace(otp))
            return false;

        if (string.IsNullOrWhiteSpace(purpose))
            throw new ArgumentException("Purpose cannot be null or empty", nameof(purpose));

        var normalizedEmail = email.ToLower();
        var lockKey = $"{OtpLockedKeyPrefix}{normalizedEmail}:{purpose}";
        var failedAttemptsKey = $"{FailedAttemptsKeyPrefix}{normalizedEmail}:{purpose}";
        var otpKey = $"{OtpKeyPrefix}{normalizedEmail}:{purpose}";

        // Check if OTP validation is locked due to too many failed attempts
        var isLocked = await _cacheService.GetAsync<bool>(lockKey);
        if (isLocked)
            return false;

        // Get stored OTP
        var storedOtp = await _cacheService.GetAsync<string>(otpKey);

        // Validate OTP
        if (string.IsNullOrEmpty(storedOtp) || !storedOtp.Equals(otp, StringComparison.Ordinal))
        {
            // Record failed attempt
            var failedAttempts = await _cacheService.GetAsync<int?>(failedAttemptsKey) ?? 0;
            failedAttempts++;

            // Set failed attempts with 15-minute expiration
            await _cacheService.SetAsync(failedAttemptsKey, failedAttempts, 
                TimeSpan.FromMinutes(FailedAttemptLockoutMinutes));

            // Lock if max attempts exceeded
            if (failedAttempts >= MaxFailedAttempts)
            {
                await _cacheService.SetAsync(lockKey, true, 
                    TimeSpan.FromMinutes(FailedAttemptLockoutMinutes));
            }

            return false;
        }

        // OTP is valid - reset failed attempts
        await _cacheService.RemoveAsync(failedAttemptsKey);
        await _cacheService.RemoveAsync(lockKey);

        return true;
    }

    /// <summary>
    /// Invalidates an OTP code by removing it from Redis.
    /// </summary>
    /// <param name="email">The email address associated with the OTP</param>
    /// <param name="purpose">The purpose of OTP</param>
    public async Task InvalidateOtpAsync(string email, string purpose)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be null or empty", nameof(email));

        if (string.IsNullOrWhiteSpace(purpose))
            throw new ArgumentException("Purpose cannot be null or empty", nameof(purpose));

        var normalizedEmail = email.ToLower();
        var otpKey = $"{OtpKeyPrefix}{normalizedEmail}:{purpose}";
        var failedAttemptsKey = $"{FailedAttemptsKeyPrefix}{normalizedEmail}:{purpose}";
        var lockKey = $"{OtpLockedKeyPrefix}{normalizedEmail}:{purpose}";

        // Remove OTP and related tracking data
        await _cacheService.RemoveAsync(otpKey);
        await _cacheService.RemoveAsync(failedAttemptsKey);
        await _cacheService.RemoveAsync(lockKey);
    }

    /// <summary>
    /// Generates a random 6-digit OTP code.
    /// </summary>
    /// <returns>A 6-digit OTP as a string</returns>
    private string GenerateOtp()
    {
        var random = new Random();
        var otp = random.Next(100000, 999999);
        return otp.ToString();
    }

    /// <summary>
    /// Gets the email subject based on OTP purpose.
    /// </summary>
  
}
