using System;
using System.Collections.Generic;

namespace LandingPageApp.Application.Exceptions
{
    /// <summary>
    /// Exception thrown when validation of input data fails.
    /// </summary>
    public class ValidationException : Exception
    {
        public Dictionary<string, string[]> Errors { get; }

        public ValidationException(string message) : base(message)
        {
            Errors = new Dictionary<string, string[]>();
        }

        public ValidationException(string message, Dictionary<string, string[]> errors) : base(message)
        {
            Errors = errors ?? new Dictionary<string, string[]>();
        }

        public ValidationException(string fieldName, string message) : base(message)
        {
            Errors = new Dictionary<string, string[]>
            {
                { fieldName, new[] { message } }
            };
        }
    }
}
