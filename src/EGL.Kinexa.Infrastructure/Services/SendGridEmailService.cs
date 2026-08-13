using EGL.Kinexa.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace EGL.Kinexa.Infrastructure.Services;

public class SendGridEmailService : IEmailService
{
    private readonly ISendGridClient _sendGridClient;
    private readonly string _fromEmail;
    private readonly string _fromName;

    public SendGridEmailService(ISendGridClient sendGridClient, IConfiguration configuration)
    {
        _sendGridClient = sendGridClient;
        _fromEmail = configuration["SendGrid:FromEmail"] ?? throw new ArgumentNullException("SendGrid:FromEmail");
        _fromName = configuration["SendGrid:FromName"] ?? throw new ArgumentNullException("SendGrid:FromName");
    }

    public async Task SendEmailAsync(string to, string subject, string htmlBody)
    {
        var from = new EmailAddress(_fromEmail, _fromName);
        var toAddress = new EmailAddress(to);
        var msg = MailHelper.CreateSingleEmail(from, toAddress, subject, string.Empty, htmlBody);
        await _sendGridClient.SendEmailAsync(msg);
    }
}
