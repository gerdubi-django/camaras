namespace NvrDesk.Application.Dtos;

public sealed class ChannelDto
{
    public Guid Id { get; set; }
    public Guid NvrDeviceId { get; set; }
    public int Number { get; set; }
    public string Name { get; set; } = string.Empty;
    public string NvrName { get; set; } = string.Empty;
}
