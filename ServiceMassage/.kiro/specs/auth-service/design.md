# Design Document - Authentication Service

## Overview

The Authentication Service is a comprehensive security layer that manages user identity, credentials, and session lifecycle. It implements JWT-based stateless authentication with refresh token rotation, OTP-based verification, and security measures including rate limiting and account lockout. The service is designed to be secure, scalable, and maintainable following SOLID principles and industry best practices.

## Architecture

The authentication system follows a layered architecture:

```
┌─────────────────────────────────────────────────────────────┐
│                    API Controllers                           │
│              (AuthController - HTTP endpoints)               │
└────────────────────────┬────────────────────────────────────┘
                         │
┌────────────────────────▼────────────────────────────────────┐
│                  Application Layer                           │
│  (AuthService - Business Logic & Orchestration)             │
└────────────────────────┬────────────────────────────────────┘
                         │
        ┌────────────────┼────────────────┐
        │                │                │
┌───────▼──────┐  ┌──────▼──────┐  ┌────▼──────────┐
│  Repository  │  │    Email    │  │  Cache/Redis  │
│   (Account)  │  │   Service   │  │   (Tokens)    │
└──────────────┘  └─────────────┘  └───────────────┘
        │                │                │
┌───────▼──────────────────────────────────▼──────────────────┐
│                  Infrastructure Layer                        │
│  (Database, Email Provider, Redis, JWT Token Generation)    │
└───────────────────────────────────────────────────────────────┘
```

## Components and Interfaces

### 1. IAuthService Interface

```csharp
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
```

### 2. ITokenService Interface

Handles JWT token generation and validation:

```csharp
public interface ITokenService
{
    string GenerateAccessToken(Account account);
    string GenerateRefreshToken();
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    bool ValidateToken(string token);
}
```

### 3. ISecurityService Interface

Handles security operations like password hashing and rate limiting:

```csharp
public interface ISecurityService
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string hash);
    Task<bool> IsAccountLockedAsync(string username);
    Task RecordFailedLoginAttemptAsync(string username);
    Task ResetFailedLoginAttemptsAsync(string username);
}
```

### 4. IOtpService Interface

Manages OTP generation and validation:

```csharp
public interface IOtpService
{
    Task<string> GenerateAndSendOtpAsync(string email, string purpose);
    Task<bool> ValidateOtpAsync(string email, string otp, string purpose);
    Task InvalidateOtpAsync(string email, string purpose);
}
```

## Data Models

### AuthResponse DTO

```csharp
public class AuthResponse
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public UserDetailDTO User { get; set; }
    public DateTime ExpiresIn { get; set; }
}
```

### UserDetailDTO

```csharp
public class UserDetailDTO
{
    public long Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string FullName { get; set; }
    public string PhoneNumber { get; set; }
    public string Address { get; set; }
    public bool Status { get; set; }
    public string Role { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastLoginAt { get; set; }
}
```

### Token Claims Structure

Access tokens contain:
- `sub` (subject): User ID
- `username`: Username
- `email`: Email address
- `role`: User role
- `iat` (issued at): Token creation time
- `exp` (expiration): Token expiration time (15 minutes)

Refresh tokens contain:
- `sub` (subject): User ID
- `type`: "refresh"
- `iat` (issued at): Token creation time
- `exp` (expiration): Token expiration time (7 days)

## Correctness Properties

A property is a characteristic or behavior that should hold true across all valid executions of a system-essentially, a formal statement about what the system should do. 
Properties serve as the bridge between human-readable specifications and machine-verifiable correctness guarantees.

### Property 1: Registration creates valid account
*For any* valid registration request with unique username and properly formatted email, registering should result in an account that can be retrieved from the database with matching username and email.
**Validates: Requirements 1.1, 1.6**

### Property 2: Duplicate username rejection
*For any* existing username, attempting to register with that username should be rejected with a conflict error, and no new account should be created.
**Validates: Requirements 1.2**

### Property 3: Password validation on registration
*For any* registration request with password shorter than 8 characters or mismatched confirmation password, the registration should be rejected with a validation error.
**Validates: Requirements 1.4, 1.5**

### Property 4: Successful login returns tokens
*For any* valid username and correct password combination, login should return both an access token and a refresh token with correct expiration times (15 minutes and 7 days respectively).
**Validates: Requirements 2.1, 2.5**

### Property 5: Failed login generic error
*For any* invalid username or incorrect password, login should return the same generic error message that does not reveal which field is incorrect.
**Validates: Requirements 2.3, 7.1**

### Property 6: Disabled account rejection
*For any* account with status set to false, login attempts should be rejected with an authorization error.
**Validates: Requirements 2.4**

### Property 7: Token refresh generates new access token
*For any* valid refresh token, calling refresh should return a new access token with updated expiration time while keeping the same user identity.
**Validates: Requirements 3.1**

### Property 8: Expired refresh token rejection
*For any* refresh token that has passed its expiration time, attempting to refresh should be rejected with an authentication error.
**Validates: Requirements 3.2**

### Property 9: Invalid token rejection
*For any* token that is malformed, tampered, or has invalid signature, the system should reject it and return an authentication error.
**Validates: Requirements 3.3, 3.4**

### Property 10: Password hash security
*For any* password, hashing the same password multiple times should produce different hashes (due to salt), but all hashes should verify against the original password.
**Validates: Requirements 1.6, 4.5**

### Property 11: OTP expiration
*For any* OTP generated for password reset, after 24 hours the OTP should be invalid and password reset should be rejected.
**Validates: Requirements 4.3, 5.4**

### Property 12: Account lockout after failed attempts
*For any* account with more than 5 failed login attempts within 15 minutes, subsequent login attempts should be rejected until the lockout period expires.
**Validates: Requirements 7.2**

### Property 13: Logout invalidates refresh token
*For any* refresh token that has been used in a logout operation, attempting to use that token for refresh should be rejected.
**Validates: Requirements 3.5, 6.1, 6.4**

### Property 14: Email verification activation
*For any* newly registered account, after submitting a valid verification OTP, the account status should change to verified and the account should be usable for login.
**Validates: Requirements 5.2**

### Property 15: User details authorization
*For any* user requesting details, if the user is not authenticated, the request should be rejected with an authentication error.
**Validates: Requirements 8.2**

### Property 16: Password hash not in response
*For any* user detail response, the password hash field should never be included in the returned data.
**Validates: Requirements 8.4**

## Error Handling

The service implements comprehensive error handling:

### Error Types

1. **ValidationException**: Invalid input data (email format, password length, etc.)
2. **AuthenticationException**: Invalid credentials or expired tokens
3. **AuthorizationException**: Insufficient permissions or disabled account
4. **ConflictException**: Resource already exists (duplicate username)
5. **NotFoundException**: Resource not found (user, token)
6. **RateLimitException**: Too many attempts (login, OTP verification)

### Error Response Format

```csharp
public class ErrorResponse
{
    public bool Success { get; set; } = false;
    public string Message { get; set; }
    public string ErrorCode { get; set; }
    public Dictionary<string, string[]> Errors { get; set; }
}
```

### Security Principles

- Never reveal whether username or password is incorrect
- Generic error messages for authentication failures
- Detailed validation errors only for client-side validation
- Log all security events internally
- Rate limit error responses to prevent enumeration attacks
