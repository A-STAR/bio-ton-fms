using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BioTonFMS.MessagesMigrations.Migrations
{
    public partial class RenameTrId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "tr_id",
                table: "tracker_messages",
                newName: "external_tracker_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "external_tracker_id",
                table: "tracker_messages",
                newName: "tr_id");
        }
    }
}
