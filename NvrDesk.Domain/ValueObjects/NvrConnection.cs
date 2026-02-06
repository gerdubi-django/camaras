using NvrDesk.Domain.Enums;

namespace NvrDesk.Domain.ValueObjects;

public sealed record NvrConnection(
    BrandType Brand,
    string Host,
    int HttpPort,
    int RtspPort,
    string Username,
    string Password);
