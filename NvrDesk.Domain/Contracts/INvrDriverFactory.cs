using NvrDesk.Domain.Enums;

namespace NvrDesk.Domain.Contracts;

public interface INvrDriverFactory
{
    INvrDriver Resolve(BrandType brand);
}
