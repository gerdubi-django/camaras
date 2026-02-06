using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NvrDesk.Infrastructure.Data;

#nullable disable

namespace NvrDesk.Infrastructure.Migrations;

[DbContext(typeof(NvrDeskDbContext))]
partial class NvrDeskDbContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
        modelBuilder.HasAnnotation("ProductVersion", "8.0.10");

        modelBuilder.Entity("NvrDesk.Domain.Entities.AuditLogEntry", b =>
        {
            b.Property<Guid>("Id").ValueGeneratedOnAdd();
            b.Property<string>("DetailsJson");
            b.Property<int>("EventType");
            b.Property<string>("Message").IsRequired().HasMaxLength(400);
            b.Property<DateTime>("TimestampUtc");
            b.HasKey("Id");
            b.ToTable("AuditLogEntries");
        });

        modelBuilder.Entity("NvrDesk.Domain.Entities.Channel", b =>
        {
            b.Property<Guid>("Id").ValueGeneratedOnAdd();
            b.Property<bool>("IsEnabled");
            b.Property<string>("Name").IsRequired().HasMaxLength(120);
            b.Property<Guid>("NvrDeviceId");
            b.Property<int>("Number");
            b.Property<string>("VendorNativeId");
            b.HasKey("Id");
            b.HasIndex("NvrDeviceId", "Number").IsUnique();
            b.ToTable("Channels");
        });

        modelBuilder.Entity("NvrDesk.Domain.Entities.NvrDevice", b =>
        {
            b.Property<Guid>("Id").ValueGeneratedOnAdd();
            b.Property<int>("Brand");
            b.Property<DateTime>("CreatedAtUtc");
            b.Property<string>("EncryptedPassword").IsRequired();
            b.Property<string>("Host").IsRequired().HasMaxLength(255);
            b.Property<int>("HttpPort");
            b.Property<string>("Name").IsRequired().HasMaxLength(120);
            b.Property<int>("RtspPort");
            b.Property<DateTime?>("UpdatedAtUtc");
            b.Property<string>("Username").IsRequired().HasMaxLength(100);
            b.HasKey("Id");
            b.ToTable("Nvrs");
        });

        modelBuilder.Entity("NvrDesk.Domain.Entities.Channel", b =>
        {
            b.HasOne("NvrDesk.Domain.Entities.NvrDevice", "NvrDevice").WithMany("Channels").HasForeignKey("NvrDeviceId").OnDelete(DeleteBehavior.Cascade).IsRequired();
            b.Navigation("NvrDevice");
        });

        modelBuilder.Entity("NvrDesk.Domain.Entities.NvrDevice", b => { b.Navigation("Channels"); });
    }
}
