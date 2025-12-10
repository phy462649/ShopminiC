using LandingPageApp.Application.Interfaces;
using LandingPageApp.Domain.Entities;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

public class EmailService : IEmailService
{
    private readonly SmtpSettings _smtp;

    public EmailService(IOptions<SmtpSettings> smtpOptions)
    {
        _smtp = smtpOptions.Value;
    }

    public async Task SendEmailAsync(string to, string subject, string bodyHtml)
    {
        bool isHtml = true;
        using var client = new SmtpClient(_smtp.Host, _smtp.Port)
        {
            EnableSsl = _smtp.EnableSsl,
            Credentials = new NetworkCredential(_smtp.User, _smtp.Password)
        };

        var mail = new MailMessage
        {
            From = new MailAddress(_smtp.FromEmail, _smtp.FromName),
            Subject = subject,
            Body = bodyHtml,
            IsBodyHtml = isHtml
        };

        mail.To.Add(to);

        try
        {
            await client.SendMailAsync(mail);
        }
        catch (SmtpException ex)
        {
            // log lỗi ở đây (logger)
            throw; // hoặc bọc thành custom exception
        }
    }

    //public async Task SendEmailWithAttachmentAsync(string to, string subject, string bodyHtml, Stream attachmentStream, string fileName, bool isHtml = true)
    //{
    //    using var client = new SmtpClient(_smtp.Host, _smtp.Port)
    //    {
    //        EnableSsl = _smtp.EnableSsl,
    //        Credentials = new NetworkCredential(_smtp.User, _smtp.Password)
    //    };

    //    var mail = new MailMessage
    //    {
    //        From = new MailAddress(_smtp.FromEmail, _smtp.FromName),
    //        Subject = subject,
    //        Body = bodyHtml,
    //        IsBodyHtml = isHtml
    //    };

    //    mail.To.Add(to);

    //    attachmentStream.Position = 0;
    //    var attachment = new Attachment(attachmentStream, fileName);
    //    mail.Attachments.Add(attachment);

    //    try
    //    {
    //        await client.SendMailAsync(mail);
    //    }
    //    catch (SmtpException)
    //    {
    //        // log và xử lý lỗi
    //        throw;
    //    }
    //}
}
