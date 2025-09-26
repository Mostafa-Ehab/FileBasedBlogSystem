using System.Net;
using System.Net.Mail;

namespace BlogSystem.Infrastructure.EmailService;

public class EmailService : IEmailService
{
    private readonly SmtpClient _smtpClient;
    private readonly string _fromAddress;

    public EmailService(string smtpHost, int smtpPort, string smtpUser, string smtpPass, string fromAddress)
    {
        _smtpClient = new SmtpClient(smtpHost)
        {
            Port = smtpPort,
            Credentials = new NetworkCredential(smtpUser, smtpPass),
            EnableSsl = true,
        };
        _fromAddress = fromAddress;
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        var mailMessage = new MailMessage(_fromAddress, to, subject, body);
        await _smtpClient.SendMailAsync(mailMessage);
    }
}