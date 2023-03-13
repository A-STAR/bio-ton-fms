using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BioTonFMSApp.Migrations
{
    public partial class RecreateTags : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tracker_tags",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    data_type = table.Column<byte>(type: "smallint", nullable: false),
                    struct_type = table.Column<int>(type: "integer", nullable: true),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
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
                    tracker_type = table.Column<int>(type: "integer", nullable: false),
                    tag_id = table.Column<int>(type: "integer", nullable: false),
                    protocol_tag_code = table.Column<int>(type: "integer", nullable: false),
                    size = table.Column<int>(type: "integer", nullable: false)
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
                    { 1, (byte)1, "Версия терминала", "term_version", null },
                    { 2, (byte)1, "Версия прошивки", "soft", null },
                    { 3, (byte)6, "IMEI", "imei", null },
                    { 4, (byte)1, "Идентификатор устройства", "device_id", null },
                    { 5, (byte)1, "Номер записи в архиве", "rec_sn", null },
                    { 6, (byte)7, "Дата и время регистрации на трекере", "tracker_date", null },
                    { 7, (byte)8, "Координаты в градусах, число спутников, признак корректности определения координат и источник координат.", "coord_struct", 1 },
                    { 8, (byte)8, "Скорость в км/ч и направление в градусах", "speed_direction", 2 },
                    { 9, (byte)1, "Высота, м", "altitude", null },
                    { 10, (byte)3, "Одно из значений:\r\n1. HDOP, если источник координат ГЛОНАСС/GPS модуль\r\n2. Погрешность в метрах, если источник базовые станции GSM-сети", "hdop", null },
                    { 11, (byte)1, "Статус устройства", "dev_status", null },
                    { 12, (byte)1, "Напряжение питания, мВ", "pwr_ext", null },
                    { 13, (byte)1, "Напряжение на батарее", "pwr_int", null },
                    { 14, (byte)1, "Температура внутри терминала, °С", "temp_int", null },
                    { 15, (byte)2, "Статус выходов", "out", null },
                    { 16, (byte)2, "Статус входов", "in", null },
                    { 17, (byte)1, "Значение на входе 0.\r\nВ зависимости от настроек один из вариантов:\r\n1. Напряжение, Мв\r\n2. число импульсов\r\n3. частота, Гц", "adc1", null },
                    { 18, (byte)1, "Значение на входе 1.\r\nВ зависимости от настроек один из вариантов:\r\n1. Напряжение, Мв\r\n2. число импульсов\r\n3. частота, Гц", "adc2", null },
                    { 19, (byte)1, "Значение на входе 2.\r\nВ зависимости от настроек один из вариантов:\r\n1. Напряжение, Мв\r\n2. число импульсов\r\n3. частота, Гц", "adc3", null },
                    { 20, (byte)1, "Значение на входе 3.\r\nВ зависимости от настроек один из вариантов:\r\n1. Напряжение, Мв\r\n2. число импульсов\r\n3. частота, Гц", "adc4", null },
                    { 21, (byte)2, "RS232 0", "adc9", null },
                    { 22, (byte)2, "RS232 1", "adc10", null },
                    { 23, (byte)1, "RS485[0]. ДУТ с адресом 0", "RS485[0]", null },
                    { 24, (byte)1, "RS485[1]. ДУТ с адресом 1", "RS485[1]", null },
                    { 25, (byte)1, "Данные CAN-шины (CAN_A0) или CAN-LOG.\r\nТопливо, израсходованное машиной с момента её создания, л", "CAN_A0", null },
                    { 26, (byte)8, "Данные CAN-шины (CAN_A1) или CAN-LOG.\r\nУровень топлива, %;\r\nтемпература охлаждающей жидкости, °C;\r\nобороты двигателя, об/мин.", "CAN_A1", 4 },
                    { 27, (byte)1, "Данные CAN-шины (CAN_B0) или CAN-LOG.\r\nПробег автомобиля, м.", "CAN_B0", null },
                    { 28, (byte)3, "CAN8BITR0\r\nили скорость транспортного средства, передаваемая с CAN-LOG’а, км/ч", "CAN8BITR0", null },
                    { 29, (byte)3, "CAN8BITR1\r\nили второй байт префикса S от CAN-LOG", "CAN8BITR1", null },
                    { 30, (byte)3, "CAN8BITR2\r\nили первый байт префикса S от CAN-LOG", "CAN8BITR2", null },
                    { 31, (byte)3, "CAN8BITR3\r\nили младший байт префикса S от CAN-LOG", "CAN8BITR3", null },
                    { 32, (byte)3, "CAN8BITR4\r\nили третий байт префикса P от CAN-LOG", "CAN8BITR4", null },
                    { 33, (byte)3, "CAN8BITR5\r\nили второй байт префикса P от CAN-LOG", "CAN8BITR5", null },
                    { 34, (byte)1, "В зависимости от настроек один из вариантов:\r\nCAN32BITR0\r\nполное время работы двигателя, ч", "CAN32BITR0", null },
                    { 35, (byte)1, "Значение на входе 4.\r\nВ зависимости от настроек один из вариантов:\r\n1. напряжение, мВ\r\n2. число импульсов\r\n3. частота, Гц", "Port 4", null },
                    { 36, (byte)1, "Значение на входе 5.\r\nВ зависимости от настроек один из вариантов:\r\n1. напряжение, мВ\r\n2. число импульсов\r\n3. частота, Гц", "Port 5", null },
                    { 37, (byte)1, "Идентификационный номер первого ключа iButton", "iButton 1", null },
                    { 38, (byte)1, "Идентификационный номер второго ключа iButton", "iButton 2", null },
                    { 39, (byte)3, "Состояние ключей iButton, идентификаторы которых заданы командой iButtons.\r\nКаждый бит соответствует одному ключу.\r\nНапример, получено: 05 или 00000101 в двоичном виде. Это значит, что подсоединены первый и третий ключи.", "iButton Keys", null },
                    { 40, (byte)2, "0 – состояние подключения к основному серверу. 1- подключен, 0 – нет.\r\n1 – статус GPRS сессии. 1- установлена, 0 – нет.\r\n2 – признак глушения GSM. 1- обнаружено глушение, 0 – нет.\r\n3 – состояние подключения к дополнительному серверу. 1 – подключен, 0 – нет.\r\n4 – признак глушения GPS/GLONASS. 1- обнаружено глушение, 0 – нет.\r\n5 – признак подключения к терминалу кабеля USB. 1 – подключен, 0 – не подключен.\r\n6 – признак наличия SD карты в терминале. 1 – присутствует, 0 – отсутствует.", "expanded_terminal_status", null }
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
                    { 34, 219, 4, 34, 1 },
                    { 35, 84, 2, 35, 1 },
                    { 36, 85, 2, 36, 1 },
                    { 37, 144, 4, 37, 1 },
                    { 38, 211, 4, 38, 1 },
                    { 39, 213, 1, 39, 1 },
                    { 40, 72, 2, 40, 1 }
                });

            migrationBuilder.CreateIndex(
                name: "ix_protocol_tag_tag_id",
                table: "protocol_tag",
                column: "tag_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "protocol_tag");

            migrationBuilder.DropTable(
                name: "tracker_tags");
        }
    }
}
