using NvrDesk.Domain.Entities;

namespace NvrDesk.Domain.Contracts;

public interface IChannelRepository
{
    Task<IReadOnlyList<Channel>> ListByNvrAsync(Guid nvrId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Channel>> ListAllAsync(CancellationToken cancellationToken = default);
    Task ReplaceForNvrAsync(Guid nvrId, IReadOnlyList<Channel> channels, CancellationToken cancellationToken = default);
}
