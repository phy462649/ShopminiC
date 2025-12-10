using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LandingPageApp.Application.Validations
{
    public class Validation
    {
        private const string EmailRegexPattern = @"^[^\s@]+@[^\s@]+\.[^\s@]+$";
        public bool IsValidEmail(string email)
        {
            try
            {
                return Regex.IsMatch(email, EmailRegexPattern, RegexOptions.IgnoreCase);
            }
            catch
            {
                return false;
            }
        }
    }
}
