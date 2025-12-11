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
        try
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

       
            await client.SendMailAsync(mail);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
    public string GetEmailSubject(string purpose)
    {
        return purpose.ToLower() switch
        {
            "registration" or "email-verification" => "Email Verification Code",
            "password-reset" => "Password Reset Code",
            _ => "Verification Code"
        };
    }

    /// <summary>
    /// Gets the email body based on OTP purpose.
    /// </summary>
    public string GetEmailBody(string otp, string purpose)
    {
        var expirationText = "15 minutes";
        return $"Your verification code is: {otp}\n\nThis code will expire in {expirationText}.\n\nIf you did not request this code, please ignore this email.";

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
