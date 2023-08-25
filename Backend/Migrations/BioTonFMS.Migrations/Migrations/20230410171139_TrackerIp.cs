using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BioTonFMS.Migrations.Migrations
{
    public partial class TrackerIp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ip_address",
                table: "trackers",
                type: "character varying(15)",
                maxLength: 15,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "last_message_received",
                table: "trackers",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "port",
                table: "trackers",
                type: "integer",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ip_address",
                table: "trackers");

            migrationBuilder.DropColumn(
                name: "last_message_received",
                table: "trackers");

            migrationBuilder.DropColumn(
                name: "port",
                table: "trackers");
        }
    }
}
