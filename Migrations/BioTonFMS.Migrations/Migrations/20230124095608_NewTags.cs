using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BioTonFMSApp.Migrations
{
    public partial class NewTags : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "tracker_tags",
                keyColumn: "id",
                keyValue: 7,
                column: "data_type",
                value: 8);

            migrationBuilder.UpdateData(
                table: "tracker_tags",
                keyColumn: "id",
                keyValue: 8,
                column: "data_type",
                value: 8);

            migrationBuilder.UpdateData(
                table: "tracker_tags",
                keyColumn: "id",
                keyValue: 26,
                column: "data_type",
                value: 8);

            migrationBuilder.InsertData(
                table: "tracker_tags",
                columns: new[] { "id", "data_type", "description", "name", "struct_type" },
                values: new object[,]
                {
                    { 35, 1, "Значение на входе 4.\nВ зависимости от настроек один из вариантов:\n1. напряжение, мВ\n2. число импульсов\n3. частота, Гц", "Port 4", null },
                    { 36, 1, "Значение на входе 5.\nВ зависимости от настроек один из вариантов:\n1. напряжение, мВ\n2. число импульсов\n3. частота, Гц", "Port 5", null },
                    { 37, 1, "Идентификационный номер первого ключа iButton", "iButton 1", null },
                    { 38, 1, "Идентификационный номер второго ключа iButton", "iButton 2", null },
                    { 39, 7, "Состояние ключей iButton, идентификаторы которых заданы командой iButtons.\nКаждый бит соответствует одному ключу.\nНапример, получено: 05 или 00000101 в двоичном виде. Это значит, что подсоединены первый и третий ключи.", "iButton Keys", null },
                    { 40, 6, "0 – состояние подключения к основному серверу. 1- подключен, 0 – нет.\n1 – статус GPRS сессии. 1- установлена, 0 – нет.\n2 – признак глушения GSM. 1- обнаружено глушение, 0 – нет.\n3 – состояние подключения к дополнительному серверу. 1 – подключен, 0 – нет.\n4 – признак глушения GPS/GLONASS. 1- обнаружено глушение, 0 – нет.\n5 – признак подключения к терминалу кабеля USB. 1 – подключен, 0 – не подключен.\n6 – признак наличия SD карты в терминале. 1 – присутствует, 0 – отсутствует.", "expanded_terminal_status", null }
                });

            migrationBuilder.InsertData(
                table: "protocol_tag",
                columns: new[] { "id", "protocol_tag_code", "size", "tag_id", "tracker_type" },
                values: new object[,]
                {
                    { 35, 84, 2, 35, 1 },
                    { 36, 85, 2, 36, 1 },
                    { 37, 144, 4, 37, 1 },
                    { 38, 211, 4, 38, 1 },
                    { 39, 213, 1, 39, 1 },
                    { 40, 72, 2, 40, 1 }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "protocol_tag",
                keyColumn: "id",
                keyValue: 35);

            migrationBuilder.DeleteData(
                table: "protocol_tag",
                keyColumn: "id",
                keyValue: 36);

            migrationBuilder.DeleteData(
                table: "protocol_tag",
                keyColumn: "id",
                keyValue: 37);

            migrationBuilder.DeleteData(
                table: "protocol_tag",
                keyColumn: "id",
                keyValue: 38);

            migrationBuilder.DeleteData(
                table: "protocol_tag",
                keyColumn: "id",
                keyValue: 39);

            migrationBuilder.DeleteData(
                table: "protocol_tag",
                keyColumn: "id",
                keyValue: 40);

            migrationBuilder.DeleteData(
                table: "tracker_tags",
                keyColumn: "id",
                keyValue: 35);

            migrationBuilder.DeleteData(
                table: "tracker_tags",
                keyColumn: "id",
                keyValue: 36);

            migrationBuilder.DeleteData(
                table: "tracker_tags",
                keyColumn: "id",
                keyValue: 37);

            migrationBuilder.DeleteData(
                table: "tracker_tags",
                keyColumn: "id",
                keyValue: 38);

            migrationBuilder.DeleteData(
                table: "tracker_tags",
                keyColumn: "id",
                keyValue: 39);

            migrationBuilder.DeleteData(
                table: "tracker_tags",
                keyColumn: "id",
                keyValue: 40);

            migrationBuilder.UpdateData(
                table: "tracker_tags",
                keyColumn: "id",
                keyValue: 7,
                column: "data_type",
                value: 7);

            migrationBuilder.UpdateData(
                table: "tracker_tags",
                keyColumn: "id",
                keyValue: 8,
                column: "data_type",
                value: 7);

            migrationBuilder.UpdateData(
                table: "tracker_tags",
                keyColumn: "id",
                keyValue: 26,
                column: "data_type",
                value: 7);
        }
    }
}
