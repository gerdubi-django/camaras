using NvrDesk.Application.Dtos;
using NvrDesk.Domain.Entities;
using NvrDesk.Domain.Enums;

namespace NvrDesk.Application.Abstractions;

public interface INvrService
{
    Task<IReadOnlyList<NvrDto>> ListNvrsAsync(CancellationToken cancellationToken = default);
    Task<(bool Success, IReadOnlyList<string> Errors)> SaveNvrAsync(NvrDto dto, CancellationToken cancellationToken = default);
    Task DeleteNvrAsync(Guid nvrId, CancellationToken cancellationToken = default);
    Task<TestResult> TestConnectionAsync(Guid nvrId, CancellationToken cancellationToken = default);
    Task<int> SyncChannelsAsync(Guid nvrId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ChannelDto>> ListAllChannelsAsync(CancellationToken cancellationToken = default);
    Task<string> BuildLiveUrlAsync(Guid channelId, StreamProfile profile, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RecordingSegmentDto>> SearchRecordingsAsync(Guid channelId, DateTime startLocal, DateTime endLocal, CancellationToken cancellationToken = default);
    Task<string> BuildPlaybackUrlAsync(Guid channelId, DateTime startLocal, DateTime endLocal, CancellationToken cancellationToken = default);
}
