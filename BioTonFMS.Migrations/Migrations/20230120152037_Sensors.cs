using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BioTonFMSApp.Migrations
{
    public partial class Sensors : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "sensors",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    tracker_id = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    data_type = table.Column<int>(type: "integer", nullable: false),
                    type_id = table.Column<int>(type: "integer", nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    formula = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    unit_id = table.Column<int>(type: "integer", nullable: false),
                    use_last_received = table.Column<bool>(type: "boolean", nullable: false),
                    validator_id = table.Column<int>(type: "integer", nullable: true),
                    validation_type = table.Column<int>(type: "integer", nullable: false),
                    fuel_use = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_sensors", x => x.id);
                    table.ForeignKey(
                        name: "fk_sensors_sensors_validator_id",
                        column: x => x.validator_id,
                        principalTable: "sensors",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_sensors_trackers_tracker_id",
                        column: x => x.tracker_id,
                        principalTable: "trackers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "trackers",
                columns: new[] { "id", "description", "external_id", "imei", "name", "sim_number", "start_date", "tracker_type" },
                values: new object[,]
                {
                    { -10, "", -10, "", "wireless", "", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0 },
                    { -9, "", -9, "", "auxiliary", "", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0 },
                    { -8, "", -8, "", "1080p", "", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0 },
                    { -7, "", -7, "", "multi-byte", "", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0 },
                    { -6, "", -6, "", "open-source", "", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0 },
                    { -5, "", -5, "", "solid state", "", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0 },
                    { -4, "", -4, "", "redundant", "", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0 },
                    { -3, "", -3, "", "wireless", "", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0 },
                    { -2, "", -2, "", "haptic", "", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0 },
                    { -1, "", -1, "", "1080p", "", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0 }
                });

            migrationBuilder.InsertData(
                table: "sensors",
                columns: new[] { "id", "data_type", "description", "formula", "fuel_use", "name", "tracker_id", "type_id", "unit_id", "use_last_received", "validation_type", "validator_id" },
                values: new object[,]
                {
                    { -100, 0, "", "someParam", 0f, "capacitor", -10, 0, 0, false, 0, null },
                    { -99, 0, "", "someParam", 0f, "panel", -10, 0, 0, false, 0, null },
                    { -98, 0, "", "someParam", 0f, "program", -10, 0, 0, false, 0, null },
                    { -97, 0, "", "someParam", 0f, "port", -10, 0, 0, false, 0, null },
                    { -96, 0, "", "someParam", 0f, "bus", -10, 0, 0, false, 0, null },
                    { -95, 0, "", "someParam", 0f, "transmitter", -10, 0, 0, false, 0, null },
                    { -94, 0, "", "someParam", 0f, "firewall", -10, 0, 0, false, 0, null },
                    { -93, 0, "", "someParam", 0f, "array", -10, 0, 0, false, 0, null },
                    { -92, 0, "", "someParam", 0f, "capacitor", -10, 0, 0, false, 0, null },
                    { -91, 0, "", "someParam", 0f, "transmitter", -10, 0, 0, false, 0, null },
                    { -90, 0, "", "someParam", 0f, "panel", -9, 0, 0, false, 0, null },
                    { -89, 0, "", "someParam", 0f, "card", -9, 0, 0, false, 0, null },
                    { -88, 0, "", "someParam", 0f, "array", -9, 0, 0, false, 0, null },
                    { -87, 0, "", "someParam", 0f, "driver", -9, 0, 0, false, 0, null },
                    { -86, 0, "", "someParam", 0f, "monitor", -9, 0, 0, false, 0, null },
                    { -85, 0, "", "someParam", 0f, "capacitor", -9, 0, 0, false, 0, null },
                    { -84, 0, "", "someParam", 0f, "transmitter", -9, 0, 0, false, 0, null },
                    { -83, 0, "", "someParam", 0f, "panel", -9, 0, 0, false, 0, null },
                    { -82, 0, "", "someParam", 0f, "panel", -9, 0, 0, false, 0, null },
                    { -81, 0, "", "someParam", 0f, "bus", -9, 0, 0, false, 0, null },
                    { -80, 0, "", "someParam", 0f, "hard drive", -8, 0, 0, false, 0, null },
                    { -79, 0, "", "someParam", 0f, "port", -8, 0, 0, false, 0, null },
                    { -78, 0, "", "someParam", 0f, "monitor", -8, 0, 0, false, 0, null },
                    { -77, 0, "", "someParam", 0f, "protocol", -8, 0, 0, false, 0, null },
                    { -76, 0, "", "someParam", 0f, "port", -8, 0, 0, false, 0, null },
                    { -75, 0, "", "someParam", 0f, "capacitor", -8, 0, 0, false, 0, null },
                    { -74, 0, "", "someParam", 0f, "monitor", -8, 0, 0, false, 0, null },
                    { -73, 0, "", "someParam", 0f, "array", -8, 0, 0, false, 0, null },
                    { -72, 0, "", "someParam", 0f, "microchip", -8, 0, 0, false, 0, null },
                    { -71, 0, "", "someParam", 0f, "hard drive", -8, 0, 0, false, 0, null },
                    { -70, 0, "", "someParam", 0f, "matrix", -7, 0, 0, false, 0, null },
                    { -69, 0, "", "someParam", 0f, "system", -7, 0, 0, false, 0, null },
                    { -68, 0, "", "someParam", 0f, "port", -7, 0, 0, false, 0, null },
                    { -67, 0, "", "someParam", 0f, "firewall", -7, 0, 0, false, 0, null },
                    { -66, 0, "", "someParam", 0f, "microchip", -7, 0, 0, false, 0, null },
                    { -65, 0, "", "someParam", 0f, "sensor", -7, 0, 0, false, 0, null },
                    { -64, 0, "", "someParam", 0f, "hard drive", -7, 0, 0, false, 0, null },
                    { -63, 0, "", "someParam", 0f, "application", -7, 0, 0, false, 0, null },
                    { -62, 0, "", "someParam", 0f, "pixel", -7, 0, 0, false, 0, null },
                    { -61, 0, "", "someParam", 0f, "pixel", -7, 0, 0, false, 0, null },
                    { -60, 0, "", "someParam", 0f, "program", -6, 0, 0, false, 0, null },
                    { -59, 0, "", "someParam", 0f, "transmitter", -6, 0, 0, false, 0, null },
                    { -58, 0, "", "someParam", 0f, "microchip", -6, 0, 0, false, 0, null },
                    { -57, 0, "", "someParam", 0f, "driver", -6, 0, 0, false, 0, null },
                    { -56, 0, "", "someParam", 0f, "driver", -6, 0, 0, false, 0, null },
                    { -55, 0, "", "someParam", 0f, "interface", -6, 0, 0, false, 0, null },
                    { -54, 0, "", "someParam", 0f, "pixel", -6, 0, 0, false, 0, null },
                    { -53, 0, "", "someParam", 0f, "interface", -6, 0, 0, false, 0, null },
                    { -52, 0, "", "someParam", 0f, "bandwidth", -6, 0, 0, false, 0, null },
                    { -51, 0, "", "someParam", 0f, "card", -6, 0, 0, false, 0, null },
                    { -50, 0, "", "someParam", 0f, "interface", -5, 0, 0, false, 0, null },
                    { -49, 0, "", "someParam", 0f, "interface", -5, 0, 0, false, 0, null },
                    { -48, 0, "", "someParam", 0f, "monitor", -5, 0, 0, false, 0, null },
                    { -47, 0, "", "someParam", 0f, "system", -5, 0, 0, false, 0, null },
                    { -46, 0, "", "someParam", 0f, "card", -5, 0, 0, false, 0, null },
                    { -45, 0, "", "someParam", 0f, "array", -5, 0, 0, false, 0, null },
                    { -44, 0, "", "someParam", 0f, "port", -5, 0, 0, false, 0, null },
                    { -43, 0, "", "someParam", 0f, "capacitor", -5, 0, 0, false, 0, null },
                    { -42, 0, "", "someParam", 0f, "firewall", -5, 0, 0, false, 0, null },
                    { -41, 0, "", "someParam", 0f, "array", -5, 0, 0, false, 0, null },
                    { -40, 0, "", "someParam", 0f, "capacitor", -4, 0, 0, false, 0, null },
                    { -39, 0, "", "someParam", 0f, "firewall", -4, 0, 0, false, 0, null },
                    { -38, 0, "", "someParam", 0f, "array", -4, 0, 0, false, 0, null },
                    { -37, 0, "", "someParam", 0f, "protocol", -4, 0, 0, false, 0, null },
                    { -36, 0, "", "someParam", 0f, "bandwidth", -4, 0, 0, false, 0, null },
                    { -35, 0, "", "someParam", 0f, "hard drive", -4, 0, 0, false, 0, null },
                    { -34, 0, "", "someParam", 0f, "program", -4, 0, 0, false, 0, null },
                    { -33, 0, "", "someParam", 0f, "program", -4, 0, 0, false, 0, null },
                    { -32, 0, "", "someParam", 0f, "system", -4, 0, 0, false, 0, null },
                    { -31, 0, "", "someParam", 0f, "bandwidth", -4, 0, 0, false, 0, null },
                    { -30, 0, "", "someParam", 0f, "matrix", -3, 0, 0, false, 0, null },
                    { -29, 0, "", "someParam", 0f, "driver", -3, 0, 0, false, 0, null },
                    { -28, 0, "", "someParam", 0f, "microchip", -3, 0, 0, false, 0, null },
                    { -27, 0, "", "someParam", 0f, "interface", -3, 0, 0, false, 0, null },
                    { -26, 0, "", "someParam", 0f, "alarm", -3, 0, 0, false, 0, null },
                    { -25, 0, "", "someParam", 0f, "microchip", -3, 0, 0, false, 0, null },
                    { -24, 0, "", "someParam", 0f, "alarm", -3, 0, 0, false, 0, null },
                    { -23, 0, "", "someParam", 0f, "circuit", -3, 0, 0, false, 0, null },
                    { -22, 0, "", "someParam", 0f, "hard drive", -3, 0, 0, false, 0, null },
                    { -21, 0, "", "someParam", 0f, "sensor", -3, 0, 0, false, 0, null },
                    { -20, 0, "", "someParam", 0f, "firewall", -2, 0, 0, false, 0, null },
                    { -19, 0, "", "someParam", 0f, "transmitter", -2, 0, 0, false, 0, null },
                    { -18, 0, "", "someParam", 0f, "card", -2, 0, 0, false, 0, null },
                    { -17, 0, "", "someParam", 0f, "alarm", -2, 0, 0, false, 0, null },
                    { -16, 0, "", "someParam", 0f, "monitor", -2, 0, 0, false, 0, null },
                    { -15, 0, "", "someParam", 0f, "feed", -2, 0, 0, false, 0, null },
                    { -14, 0, "", "someParam", 0f, "application", -2, 0, 0, false, 0, null },
                    { -13, 0, "", "someParam", 0f, "program", -2, 0, 0, false, 0, null },
                    { -12, 0, "", "someParam", 0f, "feed", -2, 0, 0, false, 0, null },
                    { -11, 0, "", "someParam", 0f, "matrix", -2, 0, 0, false, 0, null },
                    { -10, 0, "", "someParam", 0f, "port", -1, 0, 0, false, 0, null },
                    { -9, 0, "", "someParam", 0f, "matrix", -1, 0, 0, false, 0, null },
                    { -8, 0, "", "someParam", 0f, "hard drive", -1, 0, 0, false, 0, null },
                    { -7, 0, "", "someParam", 0f, "alarm", -1, 0, 0, false, 0, null },
                    { -6, 0, "", "someParam", 0f, "microchip", -1, 0, 0, false, 0, null },
                    { -5, 0, "", "someParam", 0f, "protocol", -1, 0, 0, false, 0, null },
                    { -4, 0, "", "someParam", 0f, "firewall", -1, 0, 0, false, 0, null },
                    { -3, 0, "", "someParam", 0f, "capacitor", -1, 0, 0, false, 0, null },
                    { -2, 0, "", "someParam", 0f, "hard drive", -1, 0, 0, false, 0, null },
                    { -1, 0, "", "someParam", 0f, "matrix", -1, 0, 0, false, 0, null }
                });

            migrationBuilder.CreateIndex(
                name: "ix_sensors_tracker_id",
                table: "sensors",
                column: "tracker_id");

            migrationBuilder.CreateIndex(
                name: "ix_sensors_validator_id",
                table: "sensors",
                column: "validator_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "sensors");

            migrationBuilder.DeleteData(
                table: "trackers",
                keyColumn: "id",
                keyValue: -10);

            migrationBuilder.DeleteData(
                table: "trackers",
                keyColumn: "id",
                keyValue: -9);

            migrationBuilder.DeleteData(
                table: "trackers",
                keyColumn: "id",
                keyValue: -8);

            migrationBuilder.DeleteData(
                table: "trackers",
                keyColumn: "id",
                keyValue: -7);

            migrationBuilder.DeleteData(
                table: "trackers",
                keyColumn: "id",
                keyValue: -6);

            migrationBuilder.DeleteData(
                table: "trackers",
                keyColumn: "id",
                keyValue: -5);

            migrationBuilder.DeleteData(
                table: "trackers",
                keyColumn: "id",
                keyValue: -4);

            migrationBuilder.DeleteData(
                table: "trackers",
                keyColumn: "id",
                keyValue: -3);

            migrationBuilder.DeleteData(
                table: "trackers",
                keyColumn: "id",
                keyValue: -2);

            migrationBuilder.DeleteData(
                table: "trackers",
                keyColumn: "id",
                keyValue: -1);
        }
    }
}
