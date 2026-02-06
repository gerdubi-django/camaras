using Microsoft.EntityFrameworkCore;
using NvrDesk.Domain.Contracts;
using NvrDesk.Domain.Entities;
using NvrDesk.Infrastructure.Data;

namespace NvrDesk.Infrastructure.Repositories;

public sealed class ChannelRepository(NvrDeskDbContext dbContext) : IChannelRepository
{
    public async Task<IReadOnlyList<Channel>> ListByNvrAsync(Guid nvrId, CancellationToken cancellationToken = default) =>
        await dbContext.Channels.AsNoTracking().Where(x => x.NvrDeviceId == nvrId).OrderBy(x => x.Number).ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Channel>> ListAllAsync(CancellationToken cancellationToken = default) =>
        await dbContext.Channels.AsNoTracking().OrderBy(x => x.Name).ToListAsync(cancellationToken);

    public async Task ReplaceForNvrAsync(Guid nvrId, IReadOnlyList<Channel> channels, CancellationToken cancellationToken = default)
    {
        var existing = dbContext.Channels.Where(x => x.NvrDeviceId == nvrId);
        dbContext.Channels.RemoveRange(existing);
        await dbContext.Channels.AddRangeAsync(channels, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
