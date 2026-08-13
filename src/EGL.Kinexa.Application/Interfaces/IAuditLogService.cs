using System.Threading.Tasks;

namespace EGL.Kinexa.Application.Interfaces;

public interface IAuditLogService
{
    Task LogAsync(string action, string entityName, string entityId, string userId, string? details = null);
}
