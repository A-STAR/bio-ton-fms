using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BioTonFMSApp.Migrations
{
    public partial class ChangeTrackerEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "description",
                table: "trackers",
                type: "character varying(512)",
                maxLength: 512,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "external_id",
                table: "trackers",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "sim_number",
                table: "trackers",
                type: "character varying(12)",
                maxLength: 12,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "start_date",
                table: "trackers",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "tracker_type",
                table: "trackers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "ix_trackers_external_id",
                table: "trackers",
                column: "external_id",
                unique: true);

            migrationBuilder.UpdateData(
              table: "trackers",
              keyColumn: "id",
              keyValue: 1,
              columns: new[] { "description", "sim_number", "start_date", "tracker_type" },
              values: new object[] { "tracker_description 1", "123456789121", new DateTime(2022, 10, 5, 17, 30, 50, 206, DateTimeKind.Utc).AddTicks(5208), 3 });

            migrationBuilder.UpdateData(
                table: "trackers",
                keyColumn: "id",
                keyValue: 2,
                columns: new[] { "description", "sim_number", "start_date", "tracker_type" },
                values: new object[] { "tracker_description 2", "12345678912", new DateTime(2022, 10, 5, 17, 30, 50, 206, DateTimeKind.Utc).AddTicks(5211), 3 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_trackers_external_id",
                table: "trackers");

            migrationBuilder.DropColumn(
                name: "description",
                table: "trackers");

            migrationBuilder.DropColumn(
                name: "external_id",
                table: "trackers");

            migrationBuilder.DropColumn(
                name: "sim_number",
                table: "trackers");

            migrationBuilder.DropColumn(
                name: "start_date",
                table: "trackers");

            migrationBuilder.DropColumn(
                name: "tracker_type",
                table: "trackers");
        }
    }
}
