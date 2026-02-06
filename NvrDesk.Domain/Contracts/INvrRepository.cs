using NvrDesk.Domain.Entities;

namespace NvrDesk.Domain.Contracts;

public interface INvrRepository
{
    Task<IReadOnlyList<NvrDevice>> ListAsync(CancellationToken cancellationToken = default);
    Task<NvrDevice?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(NvrDevice nvr, CancellationToken cancellationToken = default);
    Task UpdateAsync(NvrDevice nvr, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
