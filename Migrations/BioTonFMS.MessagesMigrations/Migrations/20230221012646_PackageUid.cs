using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BioTonFMS.MessagesMigrations.Migrations
{
    public partial class PackageUid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "package_uid",
                table: "tracker_messages",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "package_uid",
                table: "tracker_messages");
        }
    }
}
