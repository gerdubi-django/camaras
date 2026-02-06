using Microsoft.EntityFrameworkCore;
using NvrDesk.Domain.Contracts;
using NvrDesk.Domain.Entities;
using NvrDesk.Infrastructure.Data;

namespace NvrDesk.Infrastructure.Repositories;

public sealed class NvrRepository(NvrDeskDbContext dbContext) : INvrRepository
{
    public async Task<IReadOnlyList<NvrDevice>> ListAsync(CancellationToken cancellationToken = default) =>
        await dbContext.Nvrs.AsNoTracking().OrderBy(x => x.Name).ToListAsync(cancellationToken);

    public Task<NvrDevice?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        dbContext.Nvrs.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task AddAsync(NvrDevice nvr, CancellationToken cancellationToken = default)
    {
        await dbContext.Nvrs.AddAsync(nvr, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(NvrDevice nvr, CancellationToken cancellationToken = default)
    {
        dbContext.Nvrs.Update(nvr);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await dbContext.Nvrs.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null) return;
        dbContext.Nvrs.Remove(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
