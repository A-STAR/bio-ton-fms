using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BioTonFMS.MessagesMigrations.Migrations
{
    public partial class SensorTags : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "tracker_tag_id",
                table: "message_tags",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<bool>(
                name: "is_fallback",
                table: "message_tags",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "sensor_id",
                table: "message_tags",
                type: "integer",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_fallback",
                table: "message_tags");

            migrationBuilder.DropColumn(
                name: "sensor_id",
                table: "message_tags");

            migrationBuilder.AlterColumn<int>(
                name: "tracker_tag_id",
                table: "message_tags",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);
        }
    }
}
