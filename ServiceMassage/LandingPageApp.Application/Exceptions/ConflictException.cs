using System;

namespace LandingPageApp.Application.Exceptions
{
    /// <summary>
    /// Exception thrown when a resource already exists (e.g., duplicate username).
    /// </summary>
    public class ConflictException : Exception
    {
        public string ResourceName { get; }

        public ConflictException(string message) : base(message)
        {
            ResourceName = "Resource";
        }

        public ConflictException(string message, string resourceName) : base(message)
        {
            ResourceName = resourceName;
        }
    }
}
