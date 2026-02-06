using NvrDesk.Domain.Entities;
using NvrDesk.Domain.Enums;
using NvrDesk.Domain.ValueObjects;

namespace NvrDesk.Domain.Contracts;

public interface INvrDriver
{
    Task<TestResult> TestConnectionAsync(NvrConnection connection);
    Task<IReadOnlyList<Channel>> ListChannelsAsync(NvrConnection connection);
    string BuildRtspLiveUrl(NvrConnection connection, Channel channel, StreamProfile streamProfile);
    Task<IReadOnlyList<RecordingSegment>> SearchRecordingsAsync(NvrConnection connection, Channel channel, DateTime start, DateTime end);
    string BuildRtspPlaybackUrl(NvrConnection connection, Channel channel, DateTime start, DateTime end);
}
