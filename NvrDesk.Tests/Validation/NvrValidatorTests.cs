using NvrDesk.Application.Dtos;
using NvrDesk.Application.Validation;
using NvrDesk.Domain.Enums;

namespace NvrDesk.Tests.Validation;

public sealed class NvrValidatorTests
{
    [Fact]
    public void Validate_Should_Return_Errors_For_Missing_Fields()
    {
        var dto = new NvrDto { Brand = BrandType.Hikvision, HttpPort = 70000, RtspPort = 0 };

        var errors = NvrValidator.Validate(dto);

        Assert.True(errors.Count >= 4);
    }

    [Fact]
    public void Validate_Should_Pass_For_Valid_Dto()
    {
        var dto = new NvrDto
        {
            Name = "Oficina",
            Brand = BrandType.Dahua,
            Host = "192.168.1.20",
            HttpPort = 80,
            RtspPort = 554,
            Username = "admin",
            Password = "secret"
        };

        var errors = NvrValidator.Validate(dto);

        Assert.Empty(errors);
    }
}
