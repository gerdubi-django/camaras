using NvrDesk.Domain.Contracts;
using NvrDesk.Domain.Entities;
using NvrDesk.Infrastructure.Data;

namespace NvrDesk.Infrastructure.Repositories;

public sealed class AuditRepository(NvrDeskDbContext dbContext) : IAuditRepository
{
    public async Task AddAsync(AuditLogEntry entry, CancellationToken cancellationToken = default)
    {
        await dbContext.AuditLogEntries.AddAsync(entry, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
