using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BioTonFMS.Migrations.Migrations
{
    public partial class ChangeVoltAbbreviation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "units",
                keyColumn: "id",
                keyValue: 3,
                column: "abbreviated",
                value: "В");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "units",
                keyColumn: "id",
                keyValue: 3,
                column: "abbreviated",
                value: "V");
        }
    }
}
