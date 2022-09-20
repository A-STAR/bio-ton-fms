using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BioTonFMSApp.Migrations
{
    public partial class AlterForeignKeyInDevices : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_devices_trackers_tracker_id",
                table: "devices");

            migrationBuilder.AlterColumn<int>(
                name: "tracker_id",
                table: "devices",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "fk_devices_trackers_tracker_id",
                table: "devices",
                column: "tracker_id",
                principalTable: "trackers",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_devices_trackers_tracker_id",
                table: "devices");

            migrationBuilder.AlterColumn<int>(
                name: "tracker_id",
                table: "devices",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "fk_devices_trackers_tracker_id",
                table: "devices",
                column: "tracker_id",
                principalTable: "trackers",
                principalColumn: "id");
        }
    }
}
