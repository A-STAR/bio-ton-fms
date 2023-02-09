using System;
using System.Collections;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BioTonFMS.MessagesMigrations.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tracker_messages",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    tr_id = table.Column<int>(type: "integer", nullable: false),
                    imei = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    server_date_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    tracker_date_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    latitude = table.Column<double>(type: "double precision", nullable: true),
                    longitude = table.Column<double>(type: "double precision", nullable: true),
                    sat_number = table.Column<int>(type: "integer", nullable: true),
                    coord_correctness = table.Column<int>(type: "integer", nullable: true),
                    height = table.Column<double>(type: "double precision", nullable: true),
                    speed = table.Column<double>(type: "double precision", nullable: true),
                    direction = table.Column<double>(type: "double precision", nullable: true),
                    fuel_level = table.Column<int>(type: "integer", nullable: true),
                    coolant_temperature = table.Column<int>(type: "integer", nullable: true),
                    engine_speed = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tracker_messages", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "message_tags",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    tracker_message_id = table.Column<long>(type: "bigint", nullable: false),
                    tracker_tag_id = table.Column<int>(type: "integer", nullable: false),
                    tag_type = table.Column<byte>(type: "smallint", nullable: false),
                    value = table.Column<BitArray>(type: "bit varying(32)", maxLength: 32, nullable: true),
                    message_tag_boolean_value = table.Column<bool>(type: "boolean", nullable: true),
                    message_tag_byte_value = table.Column<byte>(type: "smallint", nullable: true),
                    message_tag_date_time_value = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    message_tag_double_value = table.Column<double>(type: "double precision", nullable: true),
                    message_tag_integer_value = table.Column<int>(type: "integer", nullable: true),
                    message_tag_string_value = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_message_tags", x => x.id);
                    table.ForeignKey(
                        name: "fk_message_tags_tracker_messages_tracker_message_id",
                        column: x => x.tracker_message_id,
                        principalTable: "tracker_messages",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_message_tags_tracker_message_id",
                table: "message_tags",
                column: "tracker_message_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "message_tags");

            migrationBuilder.DropTable(
                name: "tracker_messages");
        }
    }
}
