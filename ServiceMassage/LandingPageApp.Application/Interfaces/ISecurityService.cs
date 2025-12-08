using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LandingPageApp.Application.Interfaces
{
    public interface ISecurityService
    {
        string HashPassword(string password);
        bool VerifyPassword(string password, string hash);
        Task<bool> IsAccountLockedAsync(string username);
        Task RecordFailedLoginAttemptAsync(string username);
        Task ResetFailedLoginAttemptsAsync(string username);
    }
}
