using System.Collections.Concurrent;

namespace NvrDesk.Presentation.Services;

public sealed class StreamResourceManager(int maxConcurrentStreams)
{
    private readonly SemaphoreSlim semaphore = new(maxConcurrentStreams, maxConcurrentStreams);
    private readonly ConcurrentDictionary<Guid, byte> activeStreams = new();

    public async Task<bool> TryAcquireAsync(Guid streamId, CancellationToken cancellationToken)
    {
        await semaphore.WaitAsync(cancellationToken);
        return activeStreams.TryAdd(streamId, 1);
    }

    public void Release(Guid streamId)
    {
        if (activeStreams.TryRemove(streamId, out _)) semaphore.Release();
    }
}
