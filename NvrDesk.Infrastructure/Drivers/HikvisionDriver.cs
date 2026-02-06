using NvrDesk.Domain.Contracts;
using NvrDesk.Domain.Entities;
using NvrDesk.Domain.Enums;
using NvrDesk.Domain.ValueObjects;
using NvrDesk.Infrastructure.Http;

namespace NvrDesk.Infrastructure.Drivers;

public sealed class HikvisionDriver(NvrHttpClient client) : INvrDriver
{
    public async Task<TestResult> TestConnectionAsync(NvrConnection connection)
    {
        // This endpoint is configurable because firmware variants expose different APIs.
        var probeUri = new Uri($"http://{connection.Host}:{connection.HttpPort}/ISAPI/System/status");
        try
        {
            var ok = await client.ProbeAsync(probeUri);
            return ok ? TestResult.Success("Conexión de prueba exitosa (Hikvision).") : TestResult.Failure("No fue posible validar el endpoint Hikvision configurado.");
        }
        catch (Exception ex)
        {
            return TestResult.Failure($"Error en conexión Hikvision: {ex.Message}");
        }
    }

    public Task<IReadOnlyList<Channel>> ListChannelsAsync(NvrConnection connection)
    {
        // This is a safe stub and should be replaced by a configurable ISAPI channel parser.
        IReadOnlyList<Channel> channels = Enumerable.Range(1, 16).Select(i => new Channel { Number = i, Name = $"Canal {i}", IsEnabled = true }).ToList();
        return Task.FromResult(channels);
    }

    public string BuildRtspLiveUrl(NvrConnection connection, Channel channel, StreamProfile streamProfile)
    {
        var streamSuffix = streamProfile == StreamProfile.Sub ? "02" : "01";
        var channelCode = $"{channel.Number}{streamSuffix}";
        return $"rtsp://{Uri.EscapeDataString(connection.Username)}:{Uri.EscapeDataString(connection.Password)}@{connection.Host}:{connection.RtspPort}/Streaming/Channels/{channelCode}";
    }

    public Task<IReadOnlyList<RecordingSegment>> SearchRecordingsAsync(NvrConnection connection, Channel channel, DateTime start, DateTime end)
    {
        // This is a placeholder that returns a synthetic segment while endpoint details are configured by deployment.
        IReadOnlyList<RecordingSegment> segments = [new RecordingSegment { ChannelId = channel.Id, StartUtc = start, EndUtc = end, Quality = "Sub", Notes = "Stub Hikvision: configurar endpoint ISAPI de búsqueda." }];
        return Task.FromResult(segments);
    }

    public string BuildRtspPlaybackUrl(NvrConnection connection, Channel channel, DateTime start, DateTime end)
    {
        var from = Uri.EscapeDataString(start.ToString("yyyyMMdd'T'HHmmss'Z'"));
        var to = Uri.EscapeDataString(end.ToString("yyyyMMdd'T'HHmmss'Z'"));
        return $"rtsp://{Uri.EscapeDataString(connection.Username)}:{Uri.EscapeDataString(connection.Password)}@{connection.Host}:{connection.RtspPort}/Streaming/tracks/{channel.Number}01?starttime={from}&endtime={to}";
    }
}
