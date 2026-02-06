namespace NvrDesk.Domain.Entities;

public sealed class RecordingSegment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ChannelId { get; set; }
    public DateTime StartUtc { get; set; }
    public DateTime EndUtc { get; set; }
    public string Quality { get; set; } = "Sub";
    public string? Notes { get; set; }
}
