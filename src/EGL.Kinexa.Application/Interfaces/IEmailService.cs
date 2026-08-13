using System.Threading.Tasks;

namespace EGL.Kinexa.Application.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string htmlBody);
}
