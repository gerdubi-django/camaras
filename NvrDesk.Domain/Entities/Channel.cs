namespace NvrDesk.Domain.Entities;

public sealed class Channel
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid NvrDeviceId { get; set; }
    public int Number { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsEnabled { get; set; } = true;
    public string? VendorNativeId { get; set; }
    public NvrDevice? NvrDevice { get; set; }
}
