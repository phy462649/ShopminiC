using System;

namespace LandingPageApp.Application.Exceptions
{
    /// <summary>
    /// Exception thrown when user lacks required permissions or account is disabled.
    /// </summary>
    public class AuthorizationException : Exception
    {
        public string ErrorCode { get; }

        public AuthorizationException(string message) : base(message)
        {
            ErrorCode = "AUTHORIZATION_FAILED";
        }

        public AuthorizationException(string message, string errorCode) : base(message)
        {
            ErrorCode = errorCode;
        }
    }
}
