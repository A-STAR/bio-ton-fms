using System;
using BioTonFMS.Domain;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BioTonFMSApp.Migrations
{
    public partial class UpdateVehicleEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_vehicles_trackers_tracker_id",
                table: "vehicles");

            migrationBuilder.DropIndex(
                name: "ix_trackers_external_id",
                table: "trackers");

            migrationBuilder.DeleteData(
                table: "vehicles",
                keyColumn: "id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "vehicles",
                keyColumn: "id",
                keyValue: 2);

            migrationBuilder.AlterColumn<int>(
                name: "tracker_id",
                table: "vehicles",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "vehicles",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(128)",
                oldMaxLength: 128);

            migrationBuilder.AddColumn<string>(
                name: "description",
                table: "vehicles",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "fuel_type_id",
                table: "vehicles",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "inventory_number",
                table: "vehicles",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "make",
                table: "vehicles",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "manufacturing_year",
                table: "vehicles",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "model",
                table: "vehicles",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "registration_number",
                table: "vehicles",
                type: "character varying(15)",
                maxLength: 15,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "serial_number",
                table: "vehicles",
                type: "character varying(40)",
                maxLength: 40,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "type",
                table: "vehicles",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "vehicle_group_id",
                table: "vehicles",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "vehicle_sub_type",
                table: "vehicles",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "imei",
                table: "trackers",
                type: "character varying(15)",
                maxLength: 15,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldMaxLength: 30);

            migrationBuilder.CreateTable(
                name: "fuel_type",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_fuel_type", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "vehicle_group",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_vehicle_group", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_vehicles_fuel_type_id",
                table: "vehicles",
                column: "fuel_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_vehicles_vehicle_group_id",
                table: "vehicles",
                column: "vehicle_group_id");

            migrationBuilder.AddForeignKey(
                name: "fk_vehicles_fuel_type_fuel_type_id",
                table: "vehicles",
                column: "fuel_type_id",
                principalTable: "fuel_type",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_vehicles_trackers_tracker_id",
                table: "vehicles",
                column: "tracker_id",
                principalTable: "trackers",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_vehicles_vehicle_group_vehicle_group_id",
                table: "vehicles",
                column: "vehicle_group_id",
                principalTable: "vehicle_group",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.InsertData(
                table: "vehicle_group",
                columns: new[] { "id", "name"},
                values: new object[,]
                {
                    { 1, "Группа 1" },
                    { 2, "Группа 2" }
                });

            migrationBuilder.InsertData(
                table: "fuel_type",
                columns: new[] { "id", "name"},
                values: new object[,]
                {
                    { 1, "Бензин" },
                    { 2, "Дизельное топливо" }
                });

            migrationBuilder.InsertData(
                table: "vehicles",
                columns: new[] { "id", "name", "type", "vehicle_group_id", "make", "model", "vehicle_sub_type", "fuel_type_id", "manufacturing_year", "tracker_id" },
                values: new object[,]
                {
                    { 1, "Легковая машина", (int)VehicleTypeEnum.Transport, 1, "Ford", "Focus", (int)VehicleSubTypeEnum.Car, 1, 2019, 1},
                    { 2, "Грузовая машина", (int)VehicleTypeEnum.Transport, 1, "MAN", "TGX", (int)VehicleSubTypeEnum.Truck, 2, 2017, 2},
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_vehicles_fuel_type_fuel_type_id",
                table: "vehicles");

            migrationBuilder.DropForeignKey(
                name: "fk_vehicles_trackers_tracker_id",
                table: "vehicles");

            migrationBuilder.DropForeignKey(
                name: "fk_vehicles_vehicle_group_vehicle_group_id",
                table: "vehicles");

            migrationBuilder.DropTable(
                name: "fuel_type");

            migrationBuilder.DropTable(
                name: "vehicle_group");

            migrationBuilder.DropIndex(
                name: "ix_vehicles_fuel_type_id",
                table: "vehicles");

            migrationBuilder.DropIndex(
                name: "ix_vehicles_vehicle_group_id",
                table: "vehicles");

            migrationBuilder.DropColumn(
                name: "description",
                table: "vehicles");

            migrationBuilder.DropColumn(
                name: "fuel_type_id",
                table: "vehicles");

            migrationBuilder.DropColumn(
                name: "inventory_number",
                table: "vehicles");

            migrationBuilder.DropColumn(
                name: "make",
                table: "vehicles");

            migrationBuilder.DropColumn(
                name: "manufacturing_year",
                table: "vehicles");

            migrationBuilder.DropColumn(
                name: "model",
                table: "vehicles");

            migrationBuilder.DropColumn(
                name: "registration_number",
                table: "vehicles");

            migrationBuilder.DropColumn(
                name: "serial_number",
                table: "vehicles");

            migrationBuilder.DropColumn(
                name: "type",
                table: "vehicles");

            migrationBuilder.DropColumn(
                name: "vehicle_group_id",
                table: "vehicles");

            migrationBuilder.DropColumn(
                name: "vehicle_sub_type",
                table: "vehicles");

            migrationBuilder.AlterColumn<int>(
                name: "tracker_id",
                table: "vehicles",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "vehicles",
                type: "character varying(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "trackers",
                type: "character varying(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "imei",
                table: "trackers",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(15)",
                oldMaxLength: 15);

            migrationBuilder.AlterColumn<int>(
                name: "external_id",
                table: "trackers",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "description",
                table: "trackers",
                type: "character varying(512)",
                maxLength: 512,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500);

            migrationBuilder.AddForeignKey(
                name: "fk_vehicles_trackers_tracker_id",
                table: "vehicles",
                column: "tracker_id",
                principalTable: "trackers",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.InsertData(
                table: "trackers",
                columns: new[] { "id", "description", "external_id", "imei", "name", "sim_number", "start_date", "tracker_type" },
                values: new object[,]
                {
                                { 1, "tracker_description 1", null, "12345678", "tracker 1", "123456789121", new DateTime(2022, 10, 5, 17, 30, 50, 206, DateTimeKind.Utc).AddTicks(5208), 3 },
                                { 2, "tracker_description 2", null, "22345679", "tracker 2", "12345678912", new DateTime(2022, 10, 5, 17, 30, 50, 206, DateTimeKind.Utc).AddTicks(5211), 3 }
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

            migrationBuilder.CreateIndex(
                name: "ix_trackers_external_id",
                table: "trackers",
                column: "external_id",
                unique: true);
            
            
        }
    }
}
