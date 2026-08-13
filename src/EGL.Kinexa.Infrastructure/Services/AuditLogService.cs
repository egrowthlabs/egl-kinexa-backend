using EGL.Kinexa.Application.Interfaces;
using EGL.Kinexa.Domain.Entities;
using EGL.Kinexa.Persistence.Context;

namespace EGL.Kinexa.Infrastructure.Services;

public class AuditLogService : IAuditLogService
{
    private readonly KinexaDbContext _context;

    public AuditLogService(KinexaDbContext context)
    {
        _context = context;
    }

    public async Task LogAsync(string action, string entityName, string entityId, string userId, string? details)
    {
        var auditLog = new AuditLog
        {
            UserId = userId,
            ActionType = action,
            EntityName = entityName,
            EntityId = entityId,
            AdditionalInformation = details,
            Timestamp = DateTime.UtcNow
        };

        _context.AuditLogs.Add(auditLog);
        await _context.SaveChangesAsync();
    }
}
