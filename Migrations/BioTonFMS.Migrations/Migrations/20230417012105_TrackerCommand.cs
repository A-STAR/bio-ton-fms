using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BioTonFMS.Migrations.Migrations
{
    public partial class TrackerCommand : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tracker_command",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    tracker_id = table.Column<int>(type: "integer", nullable: false),
                    sent_date_time = table.Column<int>(type: "integer", nullable: false),
                    command_text = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    response_text = table.Column<int>(type: "integer", maxLength: 200, nullable: false),
                    binary_response = table.Column<byte[]>(type: "bytea", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tracker_command", x => x.id);
                    table.ForeignKey(
                        name: "fk_tracker_command_trackers_tracker_id",
                        column: x => x.tracker_id,
                        principalTable: "trackers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_tracker_command_tracker_id",
                table: "tracker_command",
                column: "tracker_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tracker_command");

            migrationBuilder.DropIndex(
                name: "ix_tracker_tags_name",
                table: "tracker_tags");
        }
    }
}
