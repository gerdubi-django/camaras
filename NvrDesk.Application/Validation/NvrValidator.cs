using NvrDesk.Application.Dtos;

namespace NvrDesk.Application.Validation;

public static class NvrValidator
{
    public static IReadOnlyList<string> Validate(NvrDto dto)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(dto.Name)) errors.Add("El nombre es obligatorio.");
        if (string.IsNullOrWhiteSpace(dto.Host)) errors.Add("El host es obligatorio.");
        if (dto.HttpPort is < 1 or > 65535) errors.Add("Puerto HTTP inválido.");
        if (dto.RtspPort is < 1 or > 65535) errors.Add("Puerto RTSP inválido.");
        if (string.IsNullOrWhiteSpace(dto.Username)) errors.Add("El usuario es obligatorio.");
        if (string.IsNullOrWhiteSpace(dto.Password)) errors.Add("La contraseña es obligatoria.");

        return errors;
    }
}
