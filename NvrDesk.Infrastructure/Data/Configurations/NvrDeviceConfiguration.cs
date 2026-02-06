using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NvrDesk.Domain.Entities;

namespace NvrDesk.Infrastructure.Data.Configurations;

public sealed class NvrDeviceConfiguration : IEntityTypeConfiguration<NvrDevice>
{
    public void Configure(EntityTypeBuilder<NvrDevice> builder)
    {
        builder.ToTable("Nvrs");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(120).IsRequired();
        builder.Property(x => x.Host).HasMaxLength(255).IsRequired();
        builder.Property(x => x.Username).HasMaxLength(100).IsRequired();
        builder.Property(x => x.EncryptedPassword).IsRequired();
        builder.HasMany(x => x.Channels).WithOne(x => x.NvrDevice).HasForeignKey(x => x.NvrDeviceId).OnDelete(DeleteBehavior.Cascade);
    }
}
