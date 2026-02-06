using NvrDesk.Domain.Contracts;
using NvrDesk.Domain.Enums;

namespace NvrDesk.Infrastructure.Drivers;

public sealed class NvrDriverFactory(HikvisionDriver hikvisionDriver, DahuaDriver dahuaDriver) : INvrDriverFactory
{
    public INvrDriver Resolve(BrandType brand) => brand switch
    {
        BrandType.Hikvision => hikvisionDriver,
        BrandType.Dahua => dahuaDriver,
        _ => throw new NotSupportedException($"Marca no soportada: {brand}")
    };
}
