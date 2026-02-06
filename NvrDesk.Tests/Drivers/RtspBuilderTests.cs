using NvrDesk.Domain.Entities;
using NvrDesk.Domain.Enums;
using NvrDesk.Domain.ValueObjects;
using NvrDesk.Infrastructure.Drivers;
using NvrDesk.Infrastructure.Http;

namespace NvrDesk.Tests.Drivers;

public sealed class RtspBuilderTests
{
    private static readonly NvrConnection HikConnection = new(BrandType.Hikvision, "10.0.0.50", 80, 554, "admin", "pass123");
    private static readonly NvrConnection DahuaConnection = new(BrandType.Dahua, "10.0.0.51", 80, 554, "root", "abc123");

    [Fact]
    public void Hikvision_LiveUrl_Should_Use_StreamingChannels_Format()
    {
        var driver = new HikvisionDriver(new NvrHttpClient(new HttpClient()));
        var channel = new Channel { Number = 5 };

        var url = driver.BuildRtspLiveUrl(HikConnection, channel, StreamProfile.Sub);

        Assert.Equal("rtsp://admin:pass123@10.0.0.50:554/Streaming/Channels/502", url);
    }

    [Fact]
    public void Dahua_LiveUrl_Should_Use_Realmonitor_Format()
    {
        var driver = new DahuaDriver(new NvrHttpClient(new HttpClient()));
        var channel = new Channel { Number = 2 };

        var url = driver.BuildRtspLiveUrl(DahuaConnection, channel, StreamProfile.Main);

        Assert.Equal("rtsp://root:abc123@10.0.0.51:554/cam/realmonitor?channel=2&subtype=0", url);
    }
}
