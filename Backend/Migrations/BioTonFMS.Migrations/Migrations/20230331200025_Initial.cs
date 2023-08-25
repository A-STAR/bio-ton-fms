using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BioTonFMS.Migrations.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    normalized_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    concurrency_stamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_asp_net_roles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    first_name = table.Column<string>(type: "text", nullable: false),
                    middle_name = table.Column<string>(type: "text", nullable: true),
                    last_name = table.Column<string>(type: "text", nullable: false),
                    user_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    normalized_user_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    normalized_email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    email_confirmed = table.Column<bool>(type: "boolean", nullable: false),
                    password_hash = table.Column<string>(type: "text", nullable: true),
                    security_stamp = table.Column<string>(type: "text", nullable: true),
                    concurrency_stamp = table.Column<string>(type: "text", nullable: true),
                    phone_number = table.Column<string>(type: "text", nullable: true),
                    phone_number_confirmed = table.Column<bool>(type: "boolean", nullable: false),
                    two_factor_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    lockout_end = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    lockout_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    access_failed_count = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_asp_net_users", x => x.id);
                });

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
                name: "sensor_groups",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_sensor_groups", x => x.id);
                });

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
                name: "trackers",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    external_id = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    sim_number = table.Column<string>(type: "character varying(12)", maxLength: 12, nullable: false),
                    tracker_type = table.Column<int>(type: "integer", nullable: false),
                    start_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    imei = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_trackers", x => x.id);
                });

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

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    role_id = table.Column<int>(type: "integer", nullable: false),
                    claim_type = table.Column<string>(type: "text", nullable: true),
                    claim_value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_asp_net_role_claims", x => x.id);
                    table.ForeignKey(
                        name: "fk_asp_net_role_claims_asp_net_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "AspNetRoles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    claim_type = table.Column<string>(type: "text", nullable: true),
                    claim_value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_asp_net_user_claims", x => x.id);
                    table.ForeignKey(
                        name: "fk_asp_net_user_claims_asp_net_users_user_id",
                        column: x => x.user_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    login_provider = table.Column<string>(type: "text", nullable: false),
                    provider_key = table.Column<string>(type: "text", nullable: false),
                    provider_display_name = table.Column<string>(type: "text", nullable: true),
                    user_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_asp_net_user_logins", x => new { x.login_provider, x.provider_key });
                    table.ForeignKey(
                        name: "fk_asp_net_user_logins_asp_net_users_user_id",
                        column: x => x.user_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    role_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_asp_net_user_roles", x => new { x.user_id, x.role_id });
                    table.ForeignKey(
                        name: "fk_asp_net_user_roles_asp_net_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "AspNetRoles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_asp_net_user_roles_asp_net_users_user_id",
                        column: x => x.user_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    login_provider = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_asp_net_user_tokens", x => new { x.user_id, x.login_provider, x.name });
                    table.ForeignKey(
                        name: "fk_asp_net_user_tokens_asp_net_users_user_id",
                        column: x => x.user_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "protocol_tag",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    tracker_type = table.Column<int>(type: "integer", nullable: false),
                    tag_id = table.Column<int>(type: "integer", nullable: true),
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
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "sensor_types",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false),
                    sensor_group_id = table.Column<int>(type: "integer", nullable: false),
                    data_type = table.Column<int>(type: "integer", nullable: true),
                    unit_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_sensor_types", x => x.id);
                    table.ForeignKey(
                        name: "fk_sensor_types_sensor_groups_sensor_group_id",
                        column: x => x.sensor_group_id,
                        principalTable: "sensor_groups",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_sensor_types_units_unit_id",
                        column: x => x.unit_id,
                        principalTable: "units",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "vehicles",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    type = table.Column<int>(type: "integer", nullable: false),
                    vehicle_group_id = table.Column<int>(type: "integer", nullable: true),
                    make = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    model = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    vehicle_sub_type = table.Column<int>(type: "integer", nullable: false),
                    fuel_type_id = table.Column<int>(type: "integer", nullable: false),
                    manufacturing_year = table.Column<int>(type: "integer", nullable: true),
                    registration_number = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    inventory_number = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    serial_number = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    tracker_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_vehicles", x => x.id);
                    table.ForeignKey(
                        name: "fk_vehicles_fuel_type_fuel_type_id",
                        column: x => x.fuel_type_id,
                        principalTable: "fuel_type",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_vehicles_trackers_tracker_id",
                        column: x => x.tracker_id,
                        principalTable: "trackers",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_vehicles_vehicle_group_vehicle_group_id",
                        column: x => x.vehicle_group_id,
                        principalTable: "vehicle_group",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "sensors",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    tracker_id = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    is_visible = table.Column<bool>(type: "boolean", nullable: false),
                    data_type = table.Column<int>(type: "integer", nullable: false),
                    sensor_type_id = table.Column<int>(type: "integer", nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    formula = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    unit_id = table.Column<int>(type: "integer", nullable: false),
                    use_last_received = table.Column<bool>(type: "boolean", nullable: false),
                    validator_id = table.Column<int>(type: "integer", nullable: true),
                    validation_type = table.Column<int>(type: "integer", nullable: true),
                    fuel_use = table.Column<double>(type: "double precision", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_sensors", x => x.id);
                    table.ForeignKey(
                        name: "fk_sensors_sensor_types_sensor_type_id",
                        column: x => x.sensor_type_id,
                        principalTable: "sensor_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
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
                    table.ForeignKey(
                        name: "fk_sensors_units_unit_id",
                        column: x => x.unit_id,
                        principalTable: "units",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "fuel_type",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { 1, "Бензин АИ-95" },
                    { 2, "Дизельное топливо" }
                });

            migrationBuilder.InsertData(
                table: "protocol_tag",
                columns: new[] { "id", "protocol_tag_code", "size", "tag_id", "tracker_type" },
                values: new object[,]
                {
                    { 7, 48, 9, null, 1 },
                    { 8, 51, 4, null, 1 },
                    { 26, 193, 4, null, 1 }
                });

            migrationBuilder.InsertData(
                table: "sensor_groups",
                columns: new[] { "id", "description", "name" },
                values: new object[,]
                {
                    { 1, "", "Пробег" },
                    { 2, "", "Цифровые" },
                    { 3, "", "Показатели" },
                    { 4, "", "Двигатель" },
                    { 5, "", "Топливо" },
                    { 6, "", "Другие" }
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
                    { 40, (byte)2, "0 – состояние подключения к основному серверу. 1- подключен, 0 – нет.\r\n1 – статус GPRS сессии. 1- установлена, 0 – нет.\r\n2 – признак глушения GSM. 1- обнаружено глушение, 0 – нет.\r\n3 – состояние подключения к дополнительному серверу. 1 – подключен, 0 – нет.\r\n4 – признак глушения GPS/GLONASS. 1- обнаружено глушение, 0 – нет.\r\n5 – признак подключения к терминалу кабеля USB. 1 – подключен, 0 – не подключен.\r\n6 – признак наличия SD карты в терминале. 1 – присутствует, 0 – отсутствует.", "expanded_terminal_status", null },
                    { 41, (byte)2, "", "expanded_terminal_status", null },
                    { 100, (byte)4, "Широта в градусах", "coord_latitude", null },
                    { 101, (byte)4, "Долгота в градусах", "coord_longitude", null },
                    { 102, (byte)1, "Признак корректности определения координат", "coord_correctness", null },
                    { 103, (byte)1, "Источник координат", "coord_sat_number", null },
                    { 110, (byte)1, "Уровень топлива, %", "can_log_fuel_level", null },
                    { 111, (byte)1, "Температура охлаждающей жидкости, °C", "can_log_coolant_temperature", null },
                    { 112, (byte)1, "Обороты двигателя, об/мин", "can_log_engine_speed", null },
                    { 120, (byte)4, "Скорость", "velocity_speed", null },
                    { 121, (byte)4, "Направление", "velocity_direction", null }
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

            migrationBuilder.InsertData(
                table: "vehicle_group",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { 1, "Группа 1" },
                    { 2, "Группа 2" }
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

            migrationBuilder.InsertData(
                table: "sensor_types",
                columns: new[] { "id", "data_type", "description", "name", "sensor_group_id", "unit_id" },
                values: new object[,]
                {
                    { 1, 1, "Датчик, показывающий пройденное объектом расстояние. Может использоваться в детекторе поездок для определения поездок и стоянок.", "Датчик пробега", 1, 2 },
                    { 2, 1, "Датчик, показывающий расстояние, пройденное объектом с момента получения от него последнего сообщения. Может использоваться в детекторе поездок для определения поездок и стоянок.", "Относительный одометр", 1, 2 },
                    { 3, 0, "Датчик, показывающий, включено или выключено зажигание. Может использоваться в детекторе поездок для определения поездок и стоянок и в счетчиках пробега и моточасов. Кроме того, с помощью датчика зажигания можно выявлять сливы топлива на холостом ходу. Для этого необходимо указать норму расхода топлива на холостом ходу в поле Расход, литров в час.", "Датчик зажигания", 2, 1 },
                    { 4, 1, "Датчик, ненулевое значение которого позволяет отмечать сообщение как тревожное (SOS). Сообщения, зарегистрированные в системе до создания датчика, не отмечаются как тревожные.", "Тревожная кнопка", 2, 1 },
                    { 5, 0, "Датчик, определяющий состояние движения объектов в реальном времени. Состояние движения, определенное датчиком, показывается на карте и в рабочей области вкладки Мониторинг. Для этого должны быть активированы опции Заменять иконки знаками состояния движения в настройках пользователя и Состояние движения на вкладке Мониторинг соответственно. Показания датчика могут основываться на значениях скорости, зажигания, оборотов двигателя и т. д. Как и для других типов датчиков, для него можно указать параметр в виде выражения и настроить валидацию.", "Датчик мгновенного определения движения", 2, 1 },
                    { 6, 1, "Датчик, показывающий значение напряжения. По напряжению, например, может определяться температура или состояние зажигания.", "Датчик напряжения", 3, 3 },
                    { 7, 1, "Датчик, с помощью которого можно определять массу перевозимого груза.", "Датчик веса", 3, 4 },
                    { 8, 1, "Датчик, с помощью которого можно фиксировать ускорение по осям X, Y, Z, что позволяет определять факт столкновения, т. е. дорожно-транспортного происшествия.", "Акселерометр", 3, 5 },
                    { 9, 1, "Датчик, показывающий значение температуры или какого-либо другого параметра. Датчик температуры можно использовать для анализа приходящих значений. См. пример настройки.", "Датчик температуры", 3, 6 },
                    { 10, 1, "Коэффициент, который применяется для более точных вычислений уровня топлива при разной температуре в баке. См. пример настройки.", "Коэффициент температуры", 3, 1 },
                    { 11, 1, "Датчик, показывающий частоту оборотов двигателя.", "Датчик оборотов двигателя", 4, 7 },
                    { 12, null, "Датчик, с помощью которого можно определять коэффициент движения под нагрузкой, используемый для вычисления расхода топлива по расчету. Может выступать в качестве понижающего коэффициента (при значениях от 0 до 1). Для этого датчика можно указать любую единицу измерения.", "Датчик полезной работы двигателя", 4, null },
                    { 13, 1, "Датчик, показывающий общее количество наработанных моточасов.", "Абсолютные моточасы", 4, 8 },
                    { 14, 1, "Датчик, показывающий количество моточасов с учетом коэффициента интенсивности работы. См. пример настройки.", "Относительные моточасы", 4, 8 },
                    { 15, 1, "Датчик, который показывает накапливаемое значение импульсов. Для пересчета приходящего значения в количество потраченного топлива необходимо настроить таблицу расчета и активировать опцию Рассчитывать расход топлива по датчику. Для датчиков этого типа таблица расчета применяется к разнице между двумя соседними сообщениями. Если устройство передает не накапливаемое значение импульсов, а количество импульсов между сообщениями, то необходимо использовать датчик мгновенного расхода топлива.", "Импульсный датчик расхода топлива", 5, 9 },
                    { 16, 1, "Датчик, который показывает расход топлива за весь период эксплуатации автомобиля. Чтобы получить данные о расходе топлива за конкретный период, необходимо снять показания с датчика в конце выбранного периода и вычесть показания датчика в начале периода. В свойствах датчика необходимо активировать опцию Рассчитывать расход топлива по датчику.", "Датчик абсолютного расхода топлива", 5, 9 },
                    { 17, 1, "Датчик, показывающий количество потраченного топлива с момента предыдущего измерения (сообщения). В свойствах датчика необходимо активировать опцию Рассчитывать расход топлива по датчику.", "Датчик мгновенного расхода топлива", 5, 9 },
                    { 18, 1, "Датчик, определяющий уровень топлива в баке. В свойствах датчика необходимо активировать опцию Рассчитывать расход топлива по датчику. См. пример настройки.", "Датчик уровня топлива", 5, 9 },
                    { 19, 1, "Датчик, предназначенный для расчета количества топлива в баке. При расчете разница значений импульсов из двух соседних сообщений делится на разницу времени между ними.", "Импульсный датчик уровня топлива", 5, 9 },
                    { 20, null, "Датчик, позволяющий определять интенсивность пассажиропотока или количество некоторых действий (например, открытие и закрытие двери). Есть несколько типов таких датчиков:", "Счетчик", 6, null },
                    { 21, null, "Датчик, который можно настроить для измерения любого показателя. Для произвольного датчика можно указать любую единицу измерения.", "Произвольный датчик", 6, null },
                    { 22, 2, "Датчик, с помощью которого можно фиксировать назначение водителя на объект.", "Назначение водителя", 6, 1 },
                    { 23, 2, "Датчик, с помощью которого можно фиксировать назначение прицепа на объект.", "Назначение прицепа", 6, 1 }
                });

            migrationBuilder.CreateIndex(
                name: "ix_asp_net_role_claims_role_id",
                table: "AspNetRoleClaims",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "normalized_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_asp_net_user_claims_user_id",
                table: "AspNetUserClaims",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_asp_net_user_logins_user_id",
                table: "AspNetUserLogins",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_asp_net_user_roles_role_id",
                table: "AspNetUserRoles",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "normalized_email");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "normalized_user_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_protocol_tag_tag_id",
                table: "protocol_tag",
                column: "tag_id");

            migrationBuilder.CreateIndex(
                name: "ix_sensor_types_sensor_group_id",
                table: "sensor_types",
                column: "sensor_group_id");

            migrationBuilder.CreateIndex(
                name: "ix_sensor_types_unit_id",
                table: "sensor_types",
                column: "unit_id");

            migrationBuilder.CreateIndex(
                name: "ix_sensors_sensor_type_id",
                table: "sensors",
                column: "sensor_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_sensors_tracker_id",
                table: "sensors",
                column: "tracker_id");

            migrationBuilder.CreateIndex(
                name: "ix_sensors_unit_id",
                table: "sensors",
                column: "unit_id");

            migrationBuilder.CreateIndex(
                name: "ix_sensors_validator_id",
                table: "sensors",
                column: "validator_id");

            migrationBuilder.CreateIndex(
                name: "ix_trackers_external_id",
                table: "trackers",
                column: "external_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_vehicles_fuel_type_id",
                table: "vehicles",
                column: "fuel_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_vehicles_tracker_id",
                table: "vehicles",
                column: "tracker_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_vehicles_vehicle_group_id",
                table: "vehicles",
                column: "vehicle_group_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "protocol_tag");

            migrationBuilder.DropTable(
                name: "sensors");

            migrationBuilder.DropTable(
                name: "vehicles");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "tracker_tags");

            migrationBuilder.DropTable(
                name: "sensor_types");

            migrationBuilder.DropTable(
                name: "fuel_type");

            migrationBuilder.DropTable(
                name: "trackers");

            migrationBuilder.DropTable(
                name: "vehicle_group");

            migrationBuilder.DropTable(
                name: "sensor_groups");

            migrationBuilder.DropTable(
                name: "units");
        }
    }
}
