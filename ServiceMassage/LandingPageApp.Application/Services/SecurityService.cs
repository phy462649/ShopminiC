using System;
using System.Threading.Tasks;
using BCrypt.Net;
using LandingPageApp.Application.Interfaces;

namespace LandingPageApp.Application.Services;

/// <summary>
/// SecurityService handles password hashing, verification, and account lockout logic.
/// Implements ISecurityService interface for security operations.
/// </summary>
public class SecurityService : ISecurityService
{
    private readonly ICacheRediservice _cacheService;
    private const int MaxFailedAttempts = 5;
    private const int LockoutDurationMinutes = 15;
    private const string FailedAttemptsKeyPrefix = "failed_login_attempts:";
    private const string AccountLockedKeyPrefix = "account_locked:";

    public SecurityService(ICacheRediservice cacheService)
    {
        _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
    }

    /// <summary>
    /// Hashes a password using BCrypt algorithm with automatic salt generation.
    /// </summary>
    /// <param name="password">The plaintext password to hash</param>
    /// <returns>The hashed password</returns>
    public string HashPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password cannot be null or empty", nameof(password));

        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    /// <summary>
    /// Verifies a plaintext password against a BCrypt hash.
    /// </summary>
    /// <param name="password">The plaintext password to verify</param>
    /// <param name="hash">The BCrypt hash to verify against</param>
    /// <returns>True if password matches the hash, false otherwise</returns>
    public bool VerifyPassword(string password, string hash)
    {
        if (string.IsNullOrWhiteSpace(password))
            return false;

        if (string.IsNullOrWhiteSpace(hash))
            return false;
        return BCrypt.Net.BCrypt.Verify(password, hash);
      
    }

    /// <summary>
    /// Checks if an account is currently locked due to too many failed login attempts.
    /// </summary>
    /// <param name="username">The username to check</param>
    /// <returns>True if account is locked, false otherwise</returns>
    public async Task<bool> IsAccountLockedAsync(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username cannot be null or empty", nameof(username));

        var lockKey = $"{AccountLockedKeyPrefix}{username.ToLower()}";
        var isLocked = await _cacheService.GetAsync<bool>(lockKey);
        return isLocked;
    }

    /// <summary>
    /// Records a failed login attempt for an account.
    /// Locks the account if max failed attempts is exceeded.
    /// </summary>
    /// <param name="username">The username that failed to login</param>
    public async Task RecordFailedLoginAttemptAsync(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username cannot be null or empty", nameof(username));

        var normalizedUsername = username.ToLower();
        var attemptsKey = $"{FailedAttemptsKeyPrefix}{normalizedUsername}";
        var lockKey = $"{AccountLockedKeyPrefix}{normalizedUsername}";

        // Get current attempts count
        var currentAttempts = await _cacheService.GetAsync<int>(attemptsKey);

        // Increment attempts
        currentAttempts++;

        // Check if max attempts exceeded
        if (currentAttempts >= MaxFailedAttempts)
        {
            // Lock the account
            await _cacheService.SetAsync(lockKey, true, TimeSpan.FromMinutes(LockoutDurationMinutes));
            // Reset attempts counter
            await _cacheService.RemoveAsync(attemptsKey);
        }
        else
        {
            // Store updated attempts count with expiration
            await _cacheService.SetAsync(attemptsKey, currentAttempts, TimeSpan.FromMinutes(LockoutDurationMinutes));
        }
    }

    /// <summary>
    /// Resets the failed login attempts counter for an account (typically after successful login).
    /// </summary>
    /// <param name="username">The username to reset attempts for</param>
    public async Task ResetFailedLoginAttemptsAsync(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username cannot be null or empty", nameof(username));

        var normalizedUsername = username.ToLower();
        var attemptsKey = $"{FailedAttemptsKeyPrefix}{normalizedUsername}";
        var lockKey = $"{AccountLockedKeyPrefix}{normalizedUsername}";

        // Remove both the attempts counter and lock flag
        await _cacheService.RemoveAsync(attemptsKey);
        await _cacheService.RemoveAsync(lockKey);
    }
}
