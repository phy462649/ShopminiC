using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LandingPageApp.Application.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string bodyHtml);
        string GetEmailSubject(string purpose);
        string GetEmailBody(string otp, string purpose);
    
    }
}
