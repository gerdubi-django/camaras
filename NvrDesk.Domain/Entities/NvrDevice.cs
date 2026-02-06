using NvrDesk.Domain.Enums;

namespace NvrDesk.Domain.Entities;

public sealed class NvrDevice
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public BrandType Brand { get; set; }
    public string Host { get; set; } = string.Empty;
    public int HttpPort { get; set; } = 80;
    public int RtspPort { get; set; } = 554;
    public string Username { get; set; } = string.Empty;
    public string EncryptedPassword { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAtUtc { get; set; }
    public ICollection<Channel> Channels { get; set; } = new List<Channel>();
}
