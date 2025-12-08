using LandingPageApp.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LandingPageApp.Application.Services
{
    public class EmailService : IEmailService
    {
        public Task SendMailAsync(string to, string subject, string body)
        {
            // Giả sử gửi email thành công
            Console.WriteLine("Email sent successfully to " + to);
            return Task.CompletedTask; // Trả về Task "hoàn tất"
        }
    }
}
