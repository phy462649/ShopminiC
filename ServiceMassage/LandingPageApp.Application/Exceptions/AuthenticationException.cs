using System;

namespace LandingPageApp.Application.Exceptions
{
    /// <summary>
    /// Exception thrown when authentication fails (invalid credentials, expired tokens, etc.).
    /// </summary>
    public class AuthenticationException : Exception
    {
        public string ErrorCode { get; }

        public AuthenticationException(string message) : base(message)
        {
            ErrorCode = "AUTHENTICATION_FAILED";
        }

        public AuthenticationException(string message, string errorCode) : base(message)
        {
            ErrorCode = errorCode;
        }
    }
}
