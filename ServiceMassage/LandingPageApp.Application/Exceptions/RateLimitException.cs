using System;

namespace LandingPageApp.Application.Exceptions
{
    /// <summary>
    /// Exception thrown when rate limiting is triggered (too many attempts).
    /// </summary>
    public class RateLimitException : Exception
    {
        public int RetryAfterSeconds { get; }

        public RateLimitException(string message) : base(message)
        {
            RetryAfterSeconds = 0;
        }

        public RateLimitException(string message, int retryAfterSeconds) : base(message)
        {
            RetryAfterSeconds = retryAfterSeconds;
        }
    }
}
