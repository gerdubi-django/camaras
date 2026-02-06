using Microsoft.EntityFrameworkCore;
using NvrDesk.Domain.Entities;

namespace NvrDesk.Infrastructure.Data;

public sealed class NvrDeskDbContext(DbContextOptions<NvrDeskDbContext> options) : DbContext(options)
{
    public DbSet<NvrDevice> Nvrs => Set<NvrDevice>();
    public DbSet<Channel> Channels => Set<Channel>();
    public DbSet<AuditLogEntry> AuditLogEntries => Set<AuditLogEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(NvrDeskDbContext).Assembly);
    }
}
