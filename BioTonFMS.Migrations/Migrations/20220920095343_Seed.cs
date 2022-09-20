using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BioTonFMSApp.Migrations
{
    public partial class Seed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "trackers",
                columns: new[] { "id", "imei", "name" },
                values: new object[,]
                {
                    { 1, "12345678", "tracker 1" },
                    { 2, "22345679", "tracker 2" }
                });

            migrationBuilder.InsertData(
                table: "devices",
                columns: new[] { "id", "name", "tracker_id" },
                values: new object[,]
                {
                    { 1, "D1", 2 },
                    { 2, "D2", 2 },
                    { 3, "D3", 2 }
                });

            migrationBuilder.InsertData(
                table: "vehicles",
                columns: new[] { "id", "name", "tracker_id" },
                values: new object[,]
                {
                    { 1, "vehicle 1", 1 },
                    { 2, "vehicle 2", 2 }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "devices",
                keyColumn: "id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "devices",
                keyColumn: "id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "devices",
                keyColumn: "id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "vehicles",
                keyColumn: "id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "vehicles",
                keyColumn: "id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "trackers",
                keyColumn: "id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "trackers",
                keyColumn: "id",
                keyValue: 2);
        }
    }
}
