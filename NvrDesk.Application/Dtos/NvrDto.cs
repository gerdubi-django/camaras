using NvrDesk.Domain.Enums;

namespace NvrDesk.Application.Dtos;

public sealed class NvrDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public BrandType Brand { get; set; }
    public string Host { get; set; } = string.Empty;
    public int HttpPort { get; set; }
    public int RtspPort { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
