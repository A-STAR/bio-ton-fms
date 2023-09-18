using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BioTonFMS.Migrations.Migrations
{
    public partial class AddD4Tag : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "tracker_tags",
                columns: new[] { "id", "data_type", "description", "name" },
                values: new object[] { 41, (byte)1, "Общий пробег по данным GPS/ГЛОНАСС-модулей, м.", "mileage" });

            migrationBuilder.InsertData(
                table: "protocol_tag",
                columns: new[] { "id", "protocol_tag_code", "size", "tag_id", "tracker_type" },
                values: new object[] { 41, 212, 4, 41, 1 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "protocol_tag",
                keyColumn: "id",
                keyValue: 41);

            migrationBuilder.DeleteData(
                table: "tracker_tags",
                keyColumn: "id",
                keyValue: 41);
        }
    }
}
