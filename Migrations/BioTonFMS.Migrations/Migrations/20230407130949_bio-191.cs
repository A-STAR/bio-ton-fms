using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BioTonFMS.Migrations.Migrations
{
    public partial class bio191 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "tracker_tags",
                keyColumn: "id",
                keyValue: 41);

            migrationBuilder.CreateIndex(
                name: "ix_tracker_tags_name",
                table: "tracker_tags",
                column: "name",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_tracker_tags_name",
                table: "tracker_tags");

            migrationBuilder.InsertData(
                table: "tracker_tags",
                columns: new[] { "id", "data_type", "description", "name", "struct_type" },
                values: new object[] { 41, (byte)2, "", "expanded_terminal_status", null });
        }
    }
}
