using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BioTonFMS.Migrations.Migrations
{
    public partial class Bio189RenameTrackerTags : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "struct_type",
                table: "tracker_tags");

            migrationBuilder.UpdateData(
                table: "tracker_tags",
                keyColumn: "id",
                keyValue: 10,
                column: "description",
                value: "Одно из значений:\r\n1. HDOP, если источник координат ГЛОНАСС/GPS модуль\r\n2. Погрешность в метрах, если источник базовые станции GSM-сети");

            migrationBuilder.UpdateData(
                table: "tracker_tags",
                keyColumn: "id",
                keyValue: 17,
                column: "description",
                value: "Значение на входе 0.\r\nВ зависимости от настроек один из вариантов:\r\n1. Напряжение, Мв\r\n2. число импульсов\r\n3. частота, Гц");

            migrationBuilder.UpdateData(
                table: "tracker_tags",
                keyColumn: "id",
                keyValue: 18,
                column: "description",
                value: "Значение на входе 1.\r\nВ зависимости от настроек один из вариантов:\r\n1. Напряжение, Мв\r\n2. число импульсов\r\n3. частота, Гц");

            migrationBuilder.UpdateData(
                table: "tracker_tags",
                keyColumn: "id",
                keyValue: 19,
                column: "description",
                value: "Значение на входе 2.\r\nВ зависимости от настроек один из вариантов:\r\n1. Напряжение, Мв\r\n2. число импульсов\r\n3. частота, Гц");

            migrationBuilder.UpdateData(
                table: "tracker_tags",
                keyColumn: "id",
                keyValue: 20,
                column: "description",
                value: "Значение на входе 3.\r\nВ зависимости от настроек один из вариантов:\r\n1. Напряжение, Мв\r\n2. число импульсов\r\n3. частота, Гц");

            migrationBuilder.UpdateData(
                table: "tracker_tags",
                keyColumn: "id",
                keyValue: 23,
                column: "name",
                value: "rs485_0");

            migrationBuilder.UpdateData(
                table: "tracker_tags",
                keyColumn: "id",
                keyValue: 24,
                column: "name",
                value: "rs485_1");

            migrationBuilder.UpdateData(
                table: "tracker_tags",
                keyColumn: "id",
                keyValue: 25,
                columns: new[] { "description", "name" },
                values: new object[] { "Данные CAN-шины (CAN_A0) или CAN-LOG.\r\nТопливо, израсходованное машиной с момента её создания, л", "can_a0" });

            migrationBuilder.UpdateData(
                table: "tracker_tags",
                keyColumn: "id",
                keyValue: 27,
                columns: new[] { "description", "name" },
                values: new object[] { "Данные CAN-шины (CAN_B0) или CAN-LOG.\r\nПробег автомобиля, м.", "can_b0" });

            migrationBuilder.UpdateData(
                table: "tracker_tags",
                keyColumn: "id",
                keyValue: 28,
                columns: new[] { "description", "name" },
                values: new object[] { "CAN8BITR0\r\nили скорость транспортного средства, передаваемая с CAN-LOG’а, км/ч", "can8_bitr0" });

            migrationBuilder.UpdateData(
                table: "tracker_tags",
                keyColumn: "id",
                keyValue: 29,
                columns: new[] { "description", "name" },
                values: new object[] { "CAN8BITR1\r\nили второй байт префикса S от CAN-LOG", "can8_bitr1" });

            migrationBuilder.UpdateData(
                table: "tracker_tags",
                keyColumn: "id",
                keyValue: 30,
                columns: new[] { "description", "name" },
                values: new object[] { "CAN8BITR2\r\nили первый байт префикса S от CAN-LOG", "can8_bitr2" });

            migrationBuilder.UpdateData(
                table: "tracker_tags",
                keyColumn: "id",
                keyValue: 31,
                columns: new[] { "description", "name" },
                values: new object[] { "CAN8BITR3\r\nили младший байт префикса S от CAN-LOG", "can8_bitr3" });

            migrationBuilder.UpdateData(
                table: "tracker_tags",
                keyColumn: "id",
                keyValue: 32,
                columns: new[] { "description", "name" },
                values: new object[] { "CAN8BITR4\r\nили третий байт префикса P от CAN-LOG", "can8_bitr4" });

            migrationBuilder.UpdateData(
                table: "tracker_tags",
                keyColumn: "id",
                keyValue: 33,
                columns: new[] { "description", "name" },
                values: new object[] { "CAN8BITR5\r\nили второй байт префикса P от CAN-LOG", "can8_bitr5" });

            migrationBuilder.UpdateData(
                table: "tracker_tags",
                keyColumn: "id",
                keyValue: 34,
                columns: new[] { "description", "name" },
                values: new object[] { "В зависимости от настроек один из вариантов:\r\nCAN32BITR0\r\nполное время работы двигателя, ч", "can32_bitr0" });

            migrationBuilder.UpdateData(
                table: "tracker_tags",
                keyColumn: "id",
                keyValue: 35,
                columns: new[] { "description", "name" },
                values: new object[] { "Значение на входе 4.\r\nВ зависимости от настроек один из вариантов:\r\n1. напряжение, мВ\r\n2. число импульсов\r\n3. частота, Гц", "port_4" });

            migrationBuilder.UpdateData(
                table: "tracker_tags",
                keyColumn: "id",
                keyValue: 36,
                columns: new[] { "description", "name" },
                values: new object[] { "Значение на входе 5.\r\nВ зависимости от настроек один из вариантов:\r\n1. напряжение, мВ\r\n2. число импульсов\r\n3. частота, Гц", "port_5" });

            migrationBuilder.UpdateData(
                table: "tracker_tags",
                keyColumn: "id",
                keyValue: 37,
                column: "name",
                value: "i_button_1");

            migrationBuilder.UpdateData(
                table: "tracker_tags",
                keyColumn: "id",
                keyValue: 38,
                column: "name",
                value: "i_button_2");

            migrationBuilder.UpdateData(
                table: "tracker_tags",
                keyColumn: "id",
                keyValue: 39,
                columns: new[] { "description", "name" },
                values: new object[] { "Состояние ключей iButton, идентификаторы которых заданы командой iButtons.\r\nКаждый бит соответствует одному ключу.\r\nНапример, получено: 05 или 00000101 в двоичном виде. Это значит, что подсоединены первый и третий ключи.", "i_button_keys" });

            migrationBuilder.UpdateData(
                table: "tracker_tags",
                keyColumn: "id",
                keyValue: 40,
                column: "description",
                value: "0 – состояние подключения к основному серверу. 1- подключен, 0 – нет.\r\n1 – статус GPRS сессии. 1- установлена, 0 – нет.\r\n2 – признак глушения GSM. 1- обнаружено глушение, 0 – нет.\r\n3 – состояние подключения к дополнительному серверу. 1 – подключен, 0 – нет.\r\n4 – признак глушения GPS/GLONASS. 1- обнаружено глушение, 0 – нет.\r\n5 – признак подключения к терминалу кабеля USB. 1 – подключен, 0 – не подключен.\r\n6 – признак наличия SD карты в терминале. 1 – присутствует, 0 – отсутствует.");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "struct_type",
                table: "tracker_tags",
                type: "integer",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "tracker_tags",
                keyColumn: "id",
                keyValue: 10,
                column: "description",
                value: "Одно из значений:\n1. HDOP, если источник координат ГЛОНАСС/GPS модуль\n2. Погрешность в метрах, если источник базовые станции GSM-сети");

            migrationBuilder.UpdateData(
                table: "tracker_tags",
                keyColumn: "id",
                keyValue: 17,
                column: "description",
                value: "Значение на входе 0.\nВ зависимости от настроек один из вариантов:\n1. Напряжение, Мв\n2. число импульсов\n3. частота, Гц");

            migrationBuilder.UpdateData(
                table: "tracker_tags",
                keyColumn: "id",
                keyValue: 18,
                column: "description",
                value: "Значение на входе 1.\nВ зависимости от настроек один из вариантов:\n1. Напряжение, Мв\n2. число импульсов\n3. частота, Гц");

            migrationBuilder.UpdateData(
                table: "tracker_tags",
                keyColumn: "id",
                keyValue: 19,
                column: "description",
                value: "Значение на входе 2.\nВ зависимости от настроек один из вариантов:\n1. Напряжение, Мв\n2. число импульсов\n3. частота, Гц");

            migrationBuilder.UpdateData(
                table: "tracker_tags",
                keyColumn: "id",
                keyValue: 20,
                column: "description",
                value: "Значение на входе 3.\nВ зависимости от настроек один из вариантов:\n1. Напряжение, Мв\n2. число импульсов\n3. частота, Гц");

            migrationBuilder.UpdateData(
                table: "tracker_tags",
                keyColumn: "id",
                keyValue: 23,
                column: "name",
                value: "RS485[0]");

            migrationBuilder.UpdateData(
                table: "tracker_tags",
                keyColumn: "id",
                keyValue: 24,
                column: "name",
                value: "RS485[1]");

            migrationBuilder.UpdateData(
                table: "tracker_tags",
                keyColumn: "id",
                keyValue: 25,
                columns: new[] { "description", "name" },
                values: new object[] { "Данные CAN-шины (CAN_A0) или CAN-LOG.\nТопливо, израсходованное машиной с момента её создания, л", "CAN_A0" });

            migrationBuilder.UpdateData(
                table: "tracker_tags",
                keyColumn: "id",
                keyValue: 27,
                columns: new[] { "description", "name" },
                values: new object[] { "Данные CAN-шины (CAN_B0) или CAN-LOG.\nПробег автомобиля, м.", "CAN_B0" });

            migrationBuilder.UpdateData(
                table: "tracker_tags",
                keyColumn: "id",
                keyValue: 28,
                columns: new[] { "description", "name" },
                values: new object[] { "CAN8BITR0\nили скорость транспортного средства, передаваемая с CAN-LOG’а, км/ч", "CAN8BITR0" });

            migrationBuilder.UpdateData(
                table: "tracker_tags",
                keyColumn: "id",
                keyValue: 29,
                columns: new[] { "description", "name" },
                values: new object[] { "CAN8BITR1\nили второй байт префикса S от CAN-LOG", "CAN8BITR1" });

            migrationBuilder.UpdateData(
                table: "tracker_tags",
                keyColumn: "id",
                keyValue: 30,
                columns: new[] { "description", "name" },
                values: new object[] { "CAN8BITR2\nили первый байт префикса S от CAN-LOG", "CAN8BITR2" });

            migrationBuilder.UpdateData(
                table: "tracker_tags",
                keyColumn: "id",
                keyValue: 31,
                columns: new[] { "description", "name" },
                values: new object[] { "CAN8BITR3\nили младший байт префикса S от CAN-LOG", "CAN8BITR3" });

            migrationBuilder.UpdateData(
                table: "tracker_tags",
                keyColumn: "id",
                keyValue: 32,
                columns: new[] { "description", "name" },
                values: new object[] { "CAN8BITR4\nили третий байт префикса P от CAN-LOG", "CAN8BITR4" });

            migrationBuilder.UpdateData(
                table: "tracker_tags",
                keyColumn: "id",
                keyValue: 33,
                columns: new[] { "description", "name" },
                values: new object[] { "CAN8BITR5\nили второй байт префикса P от CAN-LOG", "CAN8BITR5" });

            migrationBuilder.UpdateData(
                table: "tracker_tags",
                keyColumn: "id",
                keyValue: 34,
                columns: new[] { "description", "name" },
                values: new object[] { "В зависимости от настроек один из вариантов:\nCAN32BITR0\nполное время работы двигателя, ч", "CAN32BITR0" });

            migrationBuilder.UpdateData(
                table: "tracker_tags",
                keyColumn: "id",
                keyValue: 35,
                columns: new[] { "description", "name" },
                values: new object[] { "Значение на входе 4.\nВ зависимости от настроек один из вариантов:\n1. напряжение, мВ\n2. число импульсов\n3. частота, Гц", "Port 4" });

            migrationBuilder.UpdateData(
                table: "tracker_tags",
                keyColumn: "id",
                keyValue: 36,
                columns: new[] { "description", "name" },
                values: new object[] { "Значение на входе 5.\nВ зависимости от настроек один из вариантов:\n1. напряжение, мВ\n2. число импульсов\n3. частота, Гц", "Port 5" });

            migrationBuilder.UpdateData(
                table: "tracker_tags",
                keyColumn: "id",
                keyValue: 37,
                column: "name",
                value: "iButton 1");

            migrationBuilder.UpdateData(
                table: "tracker_tags",
                keyColumn: "id",
                keyValue: 38,
                column: "name",
                value: "iButton 2");

            migrationBuilder.UpdateData(
                table: "tracker_tags",
                keyColumn: "id",
                keyValue: 39,
                columns: new[] { "description", "name" },
                values: new object[] { "Состояние ключей iButton, идентификаторы которых заданы командой iButtons.\nКаждый бит соответствует одному ключу.\nНапример, получено: 05 или 00000101 в двоичном виде. Это значит, что подсоединены первый и третий ключи.", "iButton Keys" });

            migrationBuilder.UpdateData(
                table: "tracker_tags",
                keyColumn: "id",
                keyValue: 40,
                column: "description",
                value: "0 – состояние подключения к основному серверу. 1- подключен, 0 – нет.\n1 – статус GPRS сессии. 1- установлена, 0 – нет.\n2 – признак глушения GSM. 1- обнаружено глушение, 0 – нет.\n3 – состояние подключения к дополнительному серверу. 1 – подключен, 0 – нет.\n4 – признак глушения GPS/GLONASS. 1- обнаружено глушение, 0 – нет.\n5 – признак подключения к терминалу кабеля USB. 1 – подключен, 0 – не подключен.\n6 – признак наличия SD карты в терминале. 1 – присутствует, 0 – отсутствует.");
        }
    }
}
