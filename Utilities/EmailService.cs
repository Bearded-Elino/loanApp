using System.Net;
using System.Net.Mail;
using Loanapp.Models.Smtp;
using Microsoft.Extensions.Options;

namespace Loanapp.Utilities;

public class EmailService : IEmailService
{
    private readonly SmtpClient _smtpClient;
    private readonly string _fromAddress;

    public EmailService(IOptions<SmtpSettings> smtpSettings)
    {
        var settings = smtpSettings.Value;
        _smtpClient = new SmtpClient(settings.Host, settings.Port)
        {
            Credentials = new NetworkCredential(settings.Username, settings.Password),
            EnableSsl = true
        };
        _fromAddress = settings.FromAddress ?? throw new ArgumentNullException(nameof(settings.FromAddress), "From address cannot be null");

    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        var mailMessage = new MailMessage
        {
            From = new MailAddress(_fromAddress),
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };
        mailMessage.To.Add(to);
        await _smtpClient.SendMailAsync(mailMessage);
    }
}