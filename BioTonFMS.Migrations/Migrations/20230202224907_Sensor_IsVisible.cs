using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BioTonFMSApp.Migrations
{
    public partial class Sensor_IsVisible : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_visible",
                table: "sensors",
                type: "boolean",
                nullable: false,
                defaultValue: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_visible",
                table: "sensors");
        }
    }
}
