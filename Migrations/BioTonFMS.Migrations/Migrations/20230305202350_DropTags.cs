﻿using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BioTonFMSApp.Migrations
{
    public partial class DropTags : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "devices");

            migrationBuilder.DropTable(
                name: "protocol_tag");

            migrationBuilder.DropTable(
                name: "tracker_tags");

            migrationBuilder.DropIndex(
                name: "ix_vehicles_tracker_id",
                table: "vehicles");

            migrationBuilder.CreateIndex(
                name: "ix_vehicles_tracker_id",
                table: "vehicles",
                column: "tracker_id",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_vehicles_tracker_id",
                table: "vehicles");

            migrationBuilder.CreateTable(
                name: "devices",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    tracker_id = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_devices", x => x.id);
                    table.ForeignKey(
                        name: "fk_devices_trackers_tracker_id",
                        column: x => x.tracker_id,
                        principalTable: "trackers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tracker_tags",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    data_type = table.Column<int>(type: "integer", nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    struct_type = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tracker_tags", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "protocol_tag",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    tag_id = table.Column<int>(type: "integer", nullable: false),
                    protocol_tag_code = table.Column<int>(type: "integer", nullable: false),
                    size = table.Column<int>(type: "integer", nullable: false),
                    tracker_type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_protocol_tag", x => x.id);
                    table.ForeignKey(
                        name: "fk_protocol_tag_tracker_tags_tag_id",
                        column: x => x.tag_id,
                        principalTable: "tracker_tags",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "tracker_tags",
                columns: new[] { "id", "data_type", "description", "name", "struct_type" },
                values: new object[,]
                {
                    { 1, 1, "Версия терминала", "term_version", null },
                    { 2, 1, "Версия прошивки", "soft", null },
                    { 3, 4, "IMEI", "imei", null },
                    { 4, 1, "Идентификатор устройства", "device_id", null },
                    { 5, 1, "Номер записи в архиве", "rec_sn", null },
                    { 6, 5, "Дата и время регистрации на трекере", "tracker_date", null },
                    { 7, 7, "Координаты в градусах, число спутников, признак корректности определения координат и источник координат.", "coord_struct", 1 },
                    { 8, 7, "Скорость в км/ч и направление в градусах", "speed_direction", 2 },
                    { 9, 1, "Высота, м", "altitude", null },
                    { 10, 7, "Одно из значений:\n1. HDOP, если источник координат ГЛОНАСС/GPS модуль\n2. Погрешность в метрах, если источник базовые станции GSM-сети", "hdop", null },
                    { 11, 1, "Статус устройства", "dev_status", null },
                    { 12, 1, "Напряжение питания, мВ", "pwr_ext", null },
                    { 13, 1, "Напряжение на батарее", "pwr_int", null },
                    { 14, 1, "Температура внутри терминала, °С", "temp_int", null },
                    { 15, 6, "Статус выходов", "out", null },
                    { 16, 6, "Статус входов", "in", null },
                    { 17, 1, "Значение на входе 0.\nВ зависимости от настроек один из вариантов:\n1. Напряжение, Мв\n2. число импульсов\n3. частота, Гц", "adc1", null },
                    { 18, 1, "Значение на входе 1.\nВ зависимости от настроек один из вариантов:\n1. Напряжение, Мв\n2. число импульсов\n3. частота, Гц", "adc2", null },
                    { 19, 1, "Значение на входе 2.\nВ зависимости от настроек один из вариантов:\n1. Напряжение, Мв\n2. число импульсов\n3. частота, Гц", "adc3", null },
                    { 20, 1, "Значение на входе 3.\nВ зависимости от настроек один из вариантов:\n1. Напряжение, Мв\n2. число импульсов\n3. частота, Гц", "adc4", null },
                    { 21, 6, "RS232 0", "adc9", null },
                    { 22, 6, "RS232 1", "adc10", null },
                    { 23, 1, "RS485[0]. ДУТ с адресом 0", "RS485[0]", null },
                    { 24, 1, "RS485[1]. ДУТ с адресом 1", "RS485[1]", null },
                    { 25, 1, "Данные CAN-шины (CAN_A0) или CAN-LOG.\nТопливо, израсходованное машиной с момента её создания, л", "CAN_A0", null },
                    { 26, 7, "Данные CAN-шины (CAN_A1) или CAN-LOG.\nУровень топлива, %;\nтемпература охлаждающей жидкости, °C;\nобороты двигателя, об/мин.", "CAN_A1", 4 },
                    { 27, 1, "Данные CAN-шины (CAN_B0) или CAN-LOG.\nПробег автомобиля, м.", "CAN_B0", null },
                    { 28, 7, "CAN8BITR0\nили скорость транспортного средства, передаваемая с CAN-LOG’а, км/ч", "CAN8BITR0", null },
                    { 29, 7, "CAN8BITR1\nили второй байт префикса S от CAN-LOG", "CAN8BITR1", null },
                    { 30, 7, "CAN8BITR2\nили первый байт префикса S от CAN-LOG", "CAN8BITR2", null },
                    { 31, 7, "CAN8BITR3\nили младший байт префикса S от CAN-LOG", "CAN8BITR3", null },
                    { 32, 7, "CAN8BITR4\nили третий байт префикса P от CAN-LOG", "CAN8BITR4", null },
                    { 33, 7, "CAN8BITR5\nили второй байт префикса P от CAN-LOG", "CAN8BITR5", null },
                    { 34, 1, "В зависимости от настроек один из вариантов:\nCAN32BITR0\nполное время работы двигателя, ч", "CAN32BITR0", null }
                });

            migrationBuilder.InsertData(
                table: "protocol_tag",
                columns: new[] { "id", "protocol_tag_code", "size", "tag_id", "tracker_type" },
                values: new object[,]
                {
                    { 1, 1, 1, 1, 1 },
                    { 2, 2, 1, 2, 1 },
                    { 3, 3, 15, 3, 1 },
                    { 4, 4, 2, 4, 1 },
                    { 5, 16, 2, 5, 1 },
                    { 6, 32, 4, 6, 1 },
                    { 7, 48, 9, 7, 1 },
                    { 8, 51, 4, 8, 1 },
                    { 9, 52, 2, 9, 1 },
                    { 10, 53, 1, 10, 1 },
                    { 11, 64, 2, 11, 1 },
                    { 12, 65, 2, 12, 1 },
                    { 13, 66, 2, 13, 1 },
                    { 14, 67, 1, 14, 1 },
                    { 15, 69, 2, 15, 1 },
                    { 16, 70, 2, 16, 1 },
                    { 17, 80, 2, 17, 1 },
                    { 18, 81, 2, 18, 1 },
                    { 19, 82, 2, 19, 1 },
                    { 20, 83, 2, 20, 1 },
                    { 21, 88, 2, 21, 1 },
                    { 22, 89, 2, 22, 1 },
                    { 23, 96, 2, 23, 1 },
                    { 24, 97, 2, 24, 1 },
                    { 25, 192, 4, 25, 1 },
                    { 26, 193, 4, 26, 1 },
                    { 27, 194, 4, 27, 1 },
                    { 28, 196, 1, 28, 1 },
                    { 29, 197, 1, 29, 1 },
                    { 30, 198, 1, 30, 1 },
                    { 31, 199, 1, 31, 1 },
                    { 32, 200, 1, 32, 1 },
                    { 33, 201, 1, 33, 1 },
                    { 34, 219, 4, 34, 1 }
                });

            migrationBuilder.CreateIndex(
                name: "ix_vehicles_tracker_id",
                table: "vehicles",
                column: "tracker_id");

            migrationBuilder.CreateIndex(
                name: "ix_devices_tracker_id",
                table: "devices",
                column: "tracker_id");

            migrationBuilder.CreateIndex(
                name: "ix_protocol_tag_tag_id",
                table: "protocol_tag",
                column: "tag_id");
        }
    }
}
