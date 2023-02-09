using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BioTonFMSApp.Migrations
{
    public partial class MakeVehicleGroupNotRequired : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_vehicles_vehicle_group_vehicle_group_id",
                table: "vehicles");

            migrationBuilder.AlterColumn<int>(
                name: "vehicle_group_id",
                table: "vehicles",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "fk_vehicles_vehicle_group_vehicle_group_id",
                table: "vehicles",
                column: "vehicle_group_id",
                principalTable: "vehicle_group",
                principalColumn: "id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_vehicles_vehicle_group_vehicle_group_id",
                table: "vehicles");

            migrationBuilder.AlterColumn<int>(
                name: "vehicle_group_id",
                table: "vehicles",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "fk_vehicles_vehicle_group_vehicle_group_id",
                table: "vehicles",
                column: "vehicle_group_id",
                principalTable: "vehicle_group",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
