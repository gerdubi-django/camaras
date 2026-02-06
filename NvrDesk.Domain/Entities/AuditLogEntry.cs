using NvrDesk.Domain.Enums;

namespace NvrDesk.Domain.Entities;

public sealed class AuditLogEntry
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime TimestampUtc { get; set; } = DateTime.UtcNow;
    public AuditEventType EventType { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? DetailsJson { get; set; }
}
