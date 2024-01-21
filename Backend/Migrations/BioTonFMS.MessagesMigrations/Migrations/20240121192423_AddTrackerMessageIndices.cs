using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BioTonFMS.MessagesMigrations.Migrations
{
    public partial class AddTrackerMessageIndices : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "ix_tracker_messages_external_tracker_id",
                table: "tracker_messages",
                column: "external_tracker_id");

            migrationBuilder.CreateIndex(
                name: "ix_tracker_messages_tracker_date_time",
                table: "tracker_messages",
                column: "tracker_date_time");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_tracker_messages_external_tracker_id",
                table: "tracker_messages");

            migrationBuilder.DropIndex(
                name: "ix_tracker_messages_tracker_date_time",
                table: "tracker_messages");
        }
    }
}
