using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NvrDesk.Domain.Entities;

namespace NvrDesk.Infrastructure.Data.Configurations;

public sealed class AuditLogConfiguration : IEntityTypeConfiguration<AuditLogEntry>
{
    public void Configure(EntityTypeBuilder<AuditLogEntry> builder)
    {
        builder.ToTable("AuditLogEntries");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Message).HasMaxLength(400).IsRequired();
    }
}
