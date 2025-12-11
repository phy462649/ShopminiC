using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LandingPageApp.Application.Interfaces
{
    public interface IOtpService
    {
        Task<string> GenerateOtpAsync(string email, string purpose);
        Task<bool> ValidateOtpAsync(string email, string otp, string purpose);
        Task InvalidateOtpAsync(string email, string purpose);
    }
}
