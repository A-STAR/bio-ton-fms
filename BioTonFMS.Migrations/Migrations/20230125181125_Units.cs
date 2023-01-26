using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BioTonFMSApp.Migrations
{
    public partial class Units : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "unit",
                table: "sensors",
                newName: "unit_id");

            migrationBuilder.RenameColumn(
                name: "unit",
                table: "sensor_types",
                newName: "unit_id");

            migrationBuilder.CreateTable(
                name: "units",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    abbreviated = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_units", x => x.id);
                });

            migrationBuilder.InsertData(
                table: "units",
                columns: new[] { "id", "abbreviated", "name" },
                values: new object[,]
                {
                    { 1, "", "Безразмерная величина" },
                    { 2, "км", "Километры" },
                    { 3, "V", "Вольты" },
                    { 4, "т", "Тонны" },
                    { 5, "g", "g" },
                    { 6, "C°", "Градусы цельсия" },
                    { 7, "об/мин", "Обороты в минуту" },
                    { 8, "ч", "Часы" },
                    { 9, "л", "Литры" }
                });

            migrationBuilder.UpdateData(
                table: "sensor_types",
                keyColumn: "id",
                keyValue: 1,
                column: "unit_id",
                value: 2);

            migrationBuilder.UpdateData(
                table: "sensor_types",
                keyColumn: "id",
                keyValue: 2,
                column: "unit_id",
                value: 2);

            migrationBuilder.UpdateData(
                table: "sensor_types",
                keyColumn: "id",
                keyValue: 6,
                column: "unit_id",
                value: 3);

            migrationBuilder.UpdateData(
                table: "sensor_types",
                keyColumn: "id",
                keyValue: 7,
                column: "unit_id",
                value: 4);

            migrationBuilder.UpdateData(
                table: "sensor_types",
                keyColumn: "id",
                keyValue: 8,
                column: "unit_id",
                value: 5);

            migrationBuilder.UpdateData(
                table: "sensor_types",
                keyColumn: "id",
                keyValue: 9,
                column: "unit_id",
                value: 6);

            migrationBuilder.UpdateData(
                table: "sensor_types",
                keyColumn: "id",
                keyValue: 11,
                column: "unit_id",
                value: 7);

            migrationBuilder.UpdateData(
                table: "sensor_types",
                keyColumn: "id",
                keyValue: 13,
                column: "unit_id",
                value: 8);

            migrationBuilder.UpdateData(
                table: "sensor_types",
                keyColumn: "id",
                keyValue: 14,
                column: "unit_id",
                value: 8);

            migrationBuilder.UpdateData(
                table: "sensor_types",
                keyColumn: "id",
                keyValue: 15,
                column: "unit_id",
                value: 9);

            migrationBuilder.UpdateData(
                table: "sensor_types",
                keyColumn: "id",
                keyValue: 16,
                column: "unit_id",
                value: 9);

            migrationBuilder.UpdateData(
                table: "sensor_types",
                keyColumn: "id",
                keyValue: 17,
                column: "unit_id",
                value: 9);

            migrationBuilder.UpdateData(
                table: "sensor_types",
                keyColumn: "id",
                keyValue: 18,
                column: "unit_id",
                value: 9);

            migrationBuilder.UpdateData(
                table: "sensor_types",
                keyColumn: "id",
                keyValue: 19,
                column: "unit_id",
                value: 9);

            migrationBuilder.CreateIndex(
                name: "ix_sensors_unit_id",
                table: "sensors",
                column: "unit_id");

            migrationBuilder.CreateIndex(
                name: "ix_sensor_types_unit_id",
                table: "sensor_types",
                column: "unit_id");

            migrationBuilder.AddForeignKey(
                name: "fk_sensor_types_units_unit_id",
                table: "sensor_types",
                column: "unit_id",
                principalTable: "units",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_sensors_units_unit_id",
                table: "sensors",
                column: "unit_id",
                principalTable: "units",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_sensor_types_units_unit_id",
                table: "sensor_types");

            migrationBuilder.DropForeignKey(
                name: "fk_sensors_units_unit_id",
                table: "sensors");

            migrationBuilder.DropTable(
                name: "units");

            migrationBuilder.DropIndex(
                name: "ix_sensors_unit_id",
                table: "sensors");

            migrationBuilder.DropIndex(
                name: "ix_sensor_types_unit_id",
                table: "sensor_types");

            migrationBuilder.RenameColumn(
                name: "unit_id",
                table: "sensors",
                newName: "unit");

            migrationBuilder.RenameColumn(
                name: "unit_id",
                table: "sensor_types",
                newName: "unit");

            migrationBuilder.UpdateData(
                table: "sensor_types",
                keyColumn: "id",
                keyValue: 1,
                column: "unit",
                value: 0);

            migrationBuilder.UpdateData(
                table: "sensor_types",
                keyColumn: "id",
                keyValue: 2,
                column: "unit",
                value: 0);

            migrationBuilder.UpdateData(
                table: "sensor_types",
                keyColumn: "id",
                keyValue: 6,
                column: "unit",
                value: 2);

            migrationBuilder.UpdateData(
                table: "sensor_types",
                keyColumn: "id",
                keyValue: 7,
                column: "unit",
                value: 3);

            migrationBuilder.UpdateData(
                table: "sensor_types",
                keyColumn: "id",
                keyValue: 8,
                column: "unit",
                value: 4);

            migrationBuilder.UpdateData(
                table: "sensor_types",
                keyColumn: "id",
                keyValue: 9,
                column: "unit",
                value: 5);

            migrationBuilder.UpdateData(
                table: "sensor_types",
                keyColumn: "id",
                keyValue: 11,
                column: "unit",
                value: 6);

            migrationBuilder.UpdateData(
                table: "sensor_types",
                keyColumn: "id",
                keyValue: 13,
                column: "unit",
                value: 7);

            migrationBuilder.UpdateData(
                table: "sensor_types",
                keyColumn: "id",
                keyValue: 14,
                column: "unit",
                value: 7);

            migrationBuilder.UpdateData(
                table: "sensor_types",
                keyColumn: "id",
                keyValue: 15,
                column: "unit",
                value: 8);

            migrationBuilder.UpdateData(
                table: "sensor_types",
                keyColumn: "id",
                keyValue: 16,
                column: "unit",
                value: 8);

            migrationBuilder.UpdateData(
                table: "sensor_types",
                keyColumn: "id",
                keyValue: 17,
                column: "unit",
                value: 8);

            migrationBuilder.UpdateData(
                table: "sensor_types",
                keyColumn: "id",
                keyValue: 18,
                column: "unit",
                value: 8);

            migrationBuilder.UpdateData(
                table: "sensor_types",
                keyColumn: "id",
                keyValue: 19,
                column: "unit",
                value: 8);
        }
    }
}
