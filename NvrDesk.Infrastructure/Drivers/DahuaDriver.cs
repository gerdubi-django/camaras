using NvrDesk.Domain.Contracts;
using NvrDesk.Domain.Entities;
using NvrDesk.Domain.Enums;
using NvrDesk.Domain.ValueObjects;
using NvrDesk.Infrastructure.Http;

namespace NvrDesk.Infrastructure.Drivers;

public sealed class DahuaDriver(NvrHttpClient client) : INvrDriver
{
    public async Task<TestResult> TestConnectionAsync(NvrConnection connection)
    {
        // This endpoint is configurable because Dahua API behavior can vary by model and firmware.
        var probeUri = new Uri($"http://{connection.Host}:{connection.HttpPort}/cgi-bin/magicBox.cgi?action=getSystemInfo");
        try
        {
            var ok = await client.ProbeAsync(probeUri);
            return ok ? TestResult.Success("Conexión de prueba exitosa (Dahua).") : TestResult.Failure("No fue posible validar el endpoint Dahua configurado.");
        }
        catch (Exception ex)
        {
            return TestResult.Failure($"Error en conexión Dahua: {ex.Message}");
        }
    }

    public Task<IReadOnlyList<Channel>> ListChannelsAsync(NvrConnection connection)
    {
        // This is a safe stub and should be replaced by a configurable Dahua channel parser.
        IReadOnlyList<Channel> channels = Enumerable.Range(1, 16).Select(i => new Channel { Number = i, Name = $"Canal {i}", IsEnabled = true }).ToList();
        return Task.FromResult(channels);
    }

    public string BuildRtspLiveUrl(NvrConnection connection, Channel channel, StreamProfile streamProfile)
    {
        var subtype = streamProfile == StreamProfile.Sub ? 1 : 0;
        return $"rtsp://{Uri.EscapeDataString(connection.Username)}:{Uri.EscapeDataString(connection.Password)}@{connection.Host}:{connection.RtspPort}/cam/realmonitor?channel={channel.Number}&subtype={subtype}";
    }

    public Task<IReadOnlyList<RecordingSegment>> SearchRecordingsAsync(NvrConnection connection, Channel channel, DateTime start, DateTime end)
    {
        // This is a placeholder that returns a synthetic segment while endpoint details are configured by deployment.
        IReadOnlyList<RecordingSegment> segments = [new RecordingSegment { ChannelId = channel.Id, StartUtc = start, EndUtc = end, Quality = "Sub", Notes = "Stub Dahua: configurar endpoint CGI de búsqueda." }];
        return Task.FromResult(segments);
    }

    public string BuildRtspPlaybackUrl(NvrConnection connection, Channel channel, DateTime start, DateTime end)
    {
        var from = Uri.EscapeDataString(start.ToString("yyyy-MM-dd HH:mm:ss"));
        var to = Uri.EscapeDataString(end.ToString("yyyy-MM-dd HH:mm:ss"));
        return $"rtsp://{Uri.EscapeDataString(connection.Username)}:{Uri.EscapeDataString(connection.Password)}@{connection.Host}:{connection.RtspPort}/cam/playback?channel={channel.Number}&starttime={from}&endtime={to}";
    }
}
