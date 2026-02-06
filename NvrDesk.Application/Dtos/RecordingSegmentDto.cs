namespace NvrDesk.Application.Dtos;

public sealed class RecordingSegmentDto
{
    public DateTime StartUtc { get; set; }
    public DateTime EndUtc { get; set; }
    public string Quality { get; set; } = string.Empty;
    public string? Notes { get; set; }
}
