using NvrDesk.Application.Dtos;

namespace NvrDesk.Presentation.ViewModels;

public sealed class ChannelItemViewModel(ChannelDto dto)
{
    public Guid Id => dto.Id;
    public string DisplayName => $"{dto.NvrName} - CH {dto.Number} - {dto.Name}";
}
