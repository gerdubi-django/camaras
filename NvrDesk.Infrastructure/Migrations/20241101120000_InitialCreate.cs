using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NvrDesk.Infrastructure.Migrations;

public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "AuditLogEntries",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "TEXT", nullable: false),
                TimestampUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                EventType = table.Column<int>(type: "INTEGER", nullable: false),
                Message = table.Column<string>(type: "TEXT", maxLength: 400, nullable: false),
                DetailsJson = table.Column<string>(type: "TEXT", nullable: true)
            },
            constraints: table => table.PrimaryKey("PK_AuditLogEntries", x => x.Id));

        migrationBuilder.CreateTable(
            name: "Nvrs",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "TEXT", nullable: false),
                Name = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                Brand = table.Column<int>(type: "INTEGER", nullable: false),
                Host = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                HttpPort = table.Column<int>(type: "INTEGER", nullable: false),
                RtspPort = table.Column<int>(type: "INTEGER", nullable: false),
                Username = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                EncryptedPassword = table.Column<string>(type: "TEXT", nullable: false),
                CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                UpdatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: true)
            },
            constraints: table => table.PrimaryKey("PK_Nvrs", x => x.Id));

        migrationBuilder.CreateTable(
            name: "Channels",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "TEXT", nullable: false),
                NvrDeviceId = table.Column<Guid>(type: "TEXT", nullable: false),
                Number = table.Column<int>(type: "INTEGER", nullable: false),
                Name = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                IsEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                VendorNativeId = table.Column<string>(type: "TEXT", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Channels", x => x.Id);
                table.ForeignKey("FK_Channels_Nvrs_NvrDeviceId", x => x.NvrDeviceId, "Nvrs", "Id", onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex("IX_Channels_NvrDeviceId_Number", "Channels", new[] { "NvrDeviceId", "Number" }, unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "AuditLogEntries");
        migrationBuilder.DropTable(name: "Channels");
        migrationBuilder.DropTable(name: "Nvrs");
    }
}
