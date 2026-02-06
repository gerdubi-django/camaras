using NvrDesk.Domain.Entities;

namespace NvrDesk.Domain.Contracts;

public interface IAuditRepository
{
    Task AddAsync(AuditLogEntry entry, CancellationToken cancellationToken = default);
}
