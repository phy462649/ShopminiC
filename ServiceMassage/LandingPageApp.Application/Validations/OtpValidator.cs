using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LandingPageApp.Application.Validations
{
    public static class OtpValidator
    {
        public static void Validate(string otp)
        {
            if (string.IsNullOrWhiteSpace(otp))
                throw new ArgumentException("OTP cannot be empty");

            if (!Regex.IsMatch(otp, @"^\d{6}$"))
                throw new ArgumentException("OTP must be a 6-digit code");
        }
    }

}
