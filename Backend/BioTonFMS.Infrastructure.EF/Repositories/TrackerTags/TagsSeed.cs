using BioTonFMS.Domain;

namespace BioTonFMS.Infrastructure.EF.Repositories.TrackerTags;

public static class TagsSeed
{
    public const int LatitudeId = 100;
    public const int LongitudeId = 101;
    public const int CorrectnessId = 102;
    public const int SatNumberId = 103;

    public const int FuelLevelId = 110;
    public const int CoolantTemperatureId = 111;
    public const int EngineSpeedId = 112;
    
    public const int SpeedId = 120;
    public const int DirectionId = 121;
    
    public const int ImeiId = 3;
    public const int DeviceIdId = 4;
    public const int RecSn = 5;
    public const int TrackerDateId = 6;
    public const int AltitudeId = 9;
    public const int HdopId = 10;
    public const int DevStatusId = 11;
    public const int PwrExtId = 12;
    public const int PwrIntId = 13;
    public const int TempIntId = 14;
    public const int Adc4Id = 20;
    public const int Adc9Id = 21;
    public const int Adc10Id = 22;
    public const int Rs4850Id = 23;
    public const int Rs4851Id = 24;
    public const int CanA0Id = 25;
    public const int CanB0Id = 27;
    public const int Can8BitR0Id = 28;
    public const int Can32BitR0Id = 34;
    public const int MileageId = 41; 


    public static readonly IEnumerable<ProtocolTag> ProtocolTags = new[]
    {
        new ProtocolTag
        {
            Id = 1, TagId = 1, ProtocolTagCode = 0x01, Size = 1,
            TrackerType = TrackerTypeEnum.GalileoSkyV50
        },
        new ProtocolTag
        {
            Id = 2, TagId = 2, ProtocolTagCode = 0x02, Size = 1,
            TrackerType = TrackerTypeEnum.GalileoSkyV50
        },
        new ProtocolTag
        {
            Id = 3, TagId = 3, ProtocolTagCode = 0x03, Size = 15,
            TrackerType = TrackerTypeEnum.GalileoSkyV50
        },
        new ProtocolTag
        {
            Id = 4, TagId = 4, ProtocolTagCode = 0x04, Size = 2,
            TrackerType = TrackerTypeEnum.GalileoSkyV50
        },
        new ProtocolTag
        {
            Id = 5, TagId = 5, ProtocolTagCode = 0x10, Size = 2,
            TrackerType = TrackerTypeEnum.GalileoSkyV50
        },
        new ProtocolTag
        {
            Id = 6, TagId = 6, ProtocolTagCode = 0x20, Size = 4,
            TrackerType = TrackerTypeEnum.GalileoSkyV50
        },
        new ProtocolTag
        {
            Id = 7, ProtocolTagCode = 0x30, Size = 9,
            TrackerType = TrackerTypeEnum.GalileoSkyV50
        },
        new ProtocolTag
        {
            Id = 8, ProtocolTagCode = 0x33, Size = 4,
            TrackerType = TrackerTypeEnum.GalileoSkyV50
        },
        new ProtocolTag
        {
            Id = 9, TagId = 9, ProtocolTagCode = 0x34, Size = 2,
            TrackerType = TrackerTypeEnum.GalileoSkyV50
        },
        new ProtocolTag
        {
            Id = 10, TagId = 10, ProtocolTagCode = 0x35, Size = 1,
            TrackerType = TrackerTypeEnum.GalileoSkyV50
        },
        new ProtocolTag
        {
            Id = 11, TagId = 11, ProtocolTagCode = 0x40, Size = 2,
            TrackerType = TrackerTypeEnum.GalileoSkyV50
        },
        new ProtocolTag
        {
            Id = 12, TagId = 12, ProtocolTagCode = 0x41, Size = 2,
            TrackerType = TrackerTypeEnum.GalileoSkyV50
        },
        new ProtocolTag
        {
            Id = 13, TagId = 13, ProtocolTagCode = 0x42, Size = 2,
            TrackerType = TrackerTypeEnum.GalileoSkyV50
        },
        new ProtocolTag
        {
            Id = 14, TagId = 14, ProtocolTagCode = 0x43, Size = 1,
            TrackerType = TrackerTypeEnum.GalileoSkyV50
        },
        new ProtocolTag
        {
            Id = 15, TagId = 15, ProtocolTagCode = 0x45, Size = 2,
            TrackerType = TrackerTypeEnum.GalileoSkyV50
        },
        new ProtocolTag
        {
            Id = 16, TagId = 16, ProtocolTagCode = 0x46, Size = 2,
            TrackerType = TrackerTypeEnum.GalileoSkyV50
        },
        new ProtocolTag
        {
            Id = 17, TagId = 17, ProtocolTagCode = 0x50, Size = 2,
            TrackerType = TrackerTypeEnum.GalileoSkyV50
        },
        new ProtocolTag
        {
            Id = 18, TagId = 18, ProtocolTagCode = 0x51, Size = 2,
            TrackerType = TrackerTypeEnum.GalileoSkyV50
        },
        new ProtocolTag
        {
            Id = 19, TagId = 19, ProtocolTagCode = 0x52, Size = 2,
            TrackerType = TrackerTypeEnum.GalileoSkyV50
        },
        new ProtocolTag
        {
            Id = 20, TagId = 20, ProtocolTagCode = 0x53, Size = 2,
            TrackerType = TrackerTypeEnum.GalileoSkyV50
        },
        new ProtocolTag
        {
            Id = 21, TagId = 21, ProtocolTagCode = 0x58, Size = 2,
            TrackerType = TrackerTypeEnum.GalileoSkyV50
        },
        new ProtocolTag
        {
            Id = 22, TagId = 22, ProtocolTagCode = 0x59, Size = 2,
            TrackerType = TrackerTypeEnum.GalileoSkyV50
        },
        new ProtocolTag
        {
            Id = 23, TagId = 23, ProtocolTagCode = 0x60, Size = 2,
            TrackerType = TrackerTypeEnum.GalileoSkyV50
        },
        new ProtocolTag
        {
            Id = 24, TagId = 24, ProtocolTagCode = 0x61, Size = 2,
            TrackerType = TrackerTypeEnum.GalileoSkyV50
        },
        new ProtocolTag
        {
            Id = 25, TagId = 25, ProtocolTagCode = 0xC0, Size = 4,
            TrackerType = TrackerTypeEnum.GalileoSkyV50
        },
        new ProtocolTag
        {
            Id = 26, ProtocolTagCode = 0xC1, Size = 4,
            TrackerType = TrackerTypeEnum.GalileoSkyV50
        },
        new ProtocolTag
        {
            Id = 27, TagId = 27, ProtocolTagCode = 0xC2, Size = 4,
            TrackerType = TrackerTypeEnum.GalileoSkyV50
        },
        new ProtocolTag
        {
            Id = 28, TagId = 28, ProtocolTagCode = 0xC4, Size = 1,
            TrackerType = TrackerTypeEnum.GalileoSkyV50
        },
        new ProtocolTag
        {
            Id = 29, TagId = 29, ProtocolTagCode = 0xC5, Size = 1,
            TrackerType = TrackerTypeEnum.GalileoSkyV50
        },
        new ProtocolTag
        {
            Id = 30, TagId = 30, ProtocolTagCode = 0xC6, Size = 1,
            TrackerType = TrackerTypeEnum.GalileoSkyV50
        },
        new ProtocolTag
        {
            Id = 31, TagId = 31, ProtocolTagCode = 0xC7, Size = 1,
            TrackerType = TrackerTypeEnum.GalileoSkyV50
        },
        new ProtocolTag
        {
            Id = 32, TagId = 32, ProtocolTagCode = 0xC8, Size = 1,
            TrackerType = TrackerTypeEnum.GalileoSkyV50
        },
        new ProtocolTag
        {
            Id = 33, TagId = 33, ProtocolTagCode = 0xC9, Size = 1,
            TrackerType = TrackerTypeEnum.GalileoSkyV50
        },
        new ProtocolTag
        {
            Id = 34, TagId = 34, ProtocolTagCode = 0xDB, Size = 4,
            TrackerType = TrackerTypeEnum.GalileoSkyV50
        },
        new ProtocolTag
        {
            Id = 35, TagId = 35, ProtocolTagCode = 0x54, Size = 2,
            TrackerType = TrackerTypeEnum.GalileoSkyV50
        },
        new ProtocolTag
        {
            Id = 36, TagId = 36, ProtocolTagCode = 0x55, Size = 2,
            TrackerType = TrackerTypeEnum.GalileoSkyV50
        },
        new ProtocolTag
        {
            Id = 37, TagId = 37, ProtocolTagCode = 0x90, Size = 4,
            TrackerType = TrackerTypeEnum.GalileoSkyV50
        },
        new ProtocolTag
        {
            Id = 38, TagId = 38, ProtocolTagCode = 0xD3, Size = 4,
            TrackerType = TrackerTypeEnum.GalileoSkyV50
        },
        new ProtocolTag
        {
            Id = 39, TagId = 39, ProtocolTagCode = 0xD5, Size = 1,
            TrackerType = TrackerTypeEnum.GalileoSkyV50
        },
        new ProtocolTag
        {
            Id = 40, TagId = 40, ProtocolTagCode = 0x48, Size = 2,
            TrackerType = TrackerTypeEnum.GalileoSkyV50
        },
        new ProtocolTag
        {
            Id = MileageId, TagId = MileageId, ProtocolTagCode = 0xD4, Size = 4,
            TrackerType = TrackerTypeEnum.GalileoSkyV50
        },
    };

    public static readonly IEnumerable<TrackerTag> TrackerTags = new[]
    {
        new TrackerTag
        {
            Id = 1, Name = "term_version", DataType = TagDataTypeEnum.Integer,
            Description = "Версия терминала"
        },
        new TrackerTag
        {
            Id = 2, Name = "soft", DataType = TagDataTypeEnum.Integer,
            Description = "Версия прошивки"
        },
        new TrackerTag
        {
            Id = ImeiId, Name = "imei", DataType = TagDataTypeEnum.String,
            Description = "IMEI"
        },
        new TrackerTag
        {
            Id = DeviceIdId, Name = "device_id", DataType = TagDataTypeEnum.Integer,
            Description = "Идентификатор устройства"
        },
        new TrackerTag
        {
            Id = RecSn, Name = "rec_sn", DataType = TagDataTypeEnum.Integer,
            Description = "Номер записи в архиве"
        },
        new TrackerTag
        {
            Id = TrackerDateId, Name = "tracker_date", DataType = TagDataTypeEnum.DateTime,
            Description = "Дата и время регистрации на трекере"
        },
        new TrackerTag
        {
            Id = LatitudeId, Name = "latitude", DataType = TagDataTypeEnum.Double, Description = "Широта в градусах"
        },
        new TrackerTag
        {
            Id = LongitudeId, Name = "longitude", DataType = TagDataTypeEnum.Double, Description = "Долгота в градусах"
        },
        new TrackerTag
        {
            Id = CorrectnessId, Name = "correctness", DataType = TagDataTypeEnum.Integer,
            Description = "Признак корректности определения координат"
        },
        new TrackerTag
        {
            Id = SatNumberId, Name = "sat_number", DataType = TagDataTypeEnum.Integer, Description = "Источник координат"
        },
        new TrackerTag
        {
            Id = FuelLevelId, Name = "fuel_level", DataType = TagDataTypeEnum.Integer, Description = "Уровень топлива, %"
        },
        new TrackerTag
        {
            Id = CoolantTemperatureId, Name = "coolant_temperature", DataType = TagDataTypeEnum.Integer,
            Description = "Температура охлаждающей жидкости, °C"
        },
        new TrackerTag
        {
            Id = EngineSpeedId, Name = "engine_speed", DataType = TagDataTypeEnum.Integer, Description = "Обороты двигателя, об/мин"
        },
        new TrackerTag
        {
            Id = SpeedId, Name = "speed", DataType = TagDataTypeEnum.Double, Description = "Скорость"
        },
        new TrackerTag
        {
            Id = DirectionId, Name = "direction", DataType = TagDataTypeEnum.Double, Description = "Направление"
        },
        new TrackerTag
        {
            Id = AltitudeId, Name = "altitude", DataType = TagDataTypeEnum.Integer,
            Description = "Высота, м"
        },
        new TrackerTag
        {
            Id = HdopId, Name = "hdop", DataType = TagDataTypeEnum.Byte,
            Description = @"Одно из значений:
1. HDOP, если источник координат ГЛОНАСС/GPS модуль
2. Погрешность в метрах, если источник базовые станции GSM-сети"
        },
        new TrackerTag
        {
            Id = DevStatusId, Name = "dev_status", DataType = TagDataTypeEnum.Integer,
            Description = "Статус устройства"
        },
        new TrackerTag
        {
            Id = PwrExtId, Name = "pwr_ext", DataType = TagDataTypeEnum.Integer,
            Description = "Напряжение питания, мВ"
        },
        new TrackerTag
        {
            Id = PwrIntId, Name = "pwr_int", DataType = TagDataTypeEnum.Integer,
            Description = "Напряжение на батарее"
        },
        new TrackerTag
        {
            Id = TempIntId, Name = "temp_int", DataType = TagDataTypeEnum.Integer,
            Description = "Температура внутри терминала, °С"
        },
        new TrackerTag
        {
            Id = 15, Name = "out", DataType = TagDataTypeEnum.Bits,
            Description = "Статус выходов"
        },
        new TrackerTag
        {
            Id = 16, Name = "in", DataType = TagDataTypeEnum.Bits,
            Description = "Статус входов"
        },
        new TrackerTag
        {
            Id = 17, Name = "adc1", DataType = TagDataTypeEnum.Integer,
            Description = @"Значение на входе 0.
В зависимости от настроек один из вариантов:
1. Напряжение, Мв
2. число импульсов
3. частота, Гц"
        },
        new TrackerTag
        {
            Id = 18, Name = "adc2", DataType = TagDataTypeEnum.Integer,
            Description = @"Значение на входе 1.
В зависимости от настроек один из вариантов:
1. Напряжение, Мв
2. число импульсов
3. частота, Гц"
        },
        new TrackerTag
        {
            Id = 19, Name = "adc3", DataType = TagDataTypeEnum.Integer,
            Description = @"Значение на входе 2.
В зависимости от настроек один из вариантов:
1. Напряжение, Мв
2. число импульсов
3. частота, Гц"
        },
        new TrackerTag
        {
            Id = Adc4Id, Name = "adc4", DataType = TagDataTypeEnum.Integer,
            Description = @"Значение на входе 3.
В зависимости от настроек один из вариантов:
1. Напряжение, Мв
2. число импульсов
3. частота, Гц"
        },
        new TrackerTag
        {
            Id = Adc9Id, Name = "adc9", DataType = TagDataTypeEnum.Bits,
            Description = "RS232 0"
        },
        new TrackerTag
        {
            Id = Adc10Id, Name = "adc10", DataType = TagDataTypeEnum.Bits,
            Description = "RS232 1"
        },
        new TrackerTag
        {
            Id = Rs4850Id, Name = "rs485_0", DataType = TagDataTypeEnum.Integer,
            Description = "RS485[0]. ДУТ с адресом 0"
        },
        new TrackerTag
        {
            Id = Rs4851Id, Name = "rs485_1", DataType = TagDataTypeEnum.Integer,
            Description = "RS485[1]. ДУТ с адресом 1"
        },
        new TrackerTag
        {
            Id = CanA0Id, Name = "can_a0", DataType = TagDataTypeEnum.Integer,
            Description = @"Данные CAN-шины (CAN_A0) или CAN-LOG.
Топливо, израсходованное машиной с момента её создания, л"
        },
        new TrackerTag
        {
            Id = 27, Name = "can_b0", DataType = TagDataTypeEnum.Integer,
            Description = @"Данные CAN-шины (CAN_B0) или CAN-LOG.
Пробег автомобиля, м."
        },
        new TrackerTag
        {
            Id = Can8BitR0Id, Name = "can8_bitr0", DataType = TagDataTypeEnum.Byte,
            Description = @"CAN8BITR0
или скорость транспортного средства, передаваемая с CAN-LOG’а, км/ч"
        },
        new TrackerTag
        {
            Id = 29, Name = "can8_bitr1", DataType = TagDataTypeEnum.Byte,
            Description = @"CAN8BITR1
или второй байт префикса S от CAN-LOG"
        },
        new TrackerTag
        {
            Id = 30, Name = "can8_bitr2", DataType = TagDataTypeEnum.Byte,
            Description = @"CAN8BITR2
или первый байт префикса S от CAN-LOG"
        },
        new TrackerTag
        {
            Id = 31, Name = "can8_bitr3", DataType = TagDataTypeEnum.Byte,
            Description = @"CAN8BITR3
или младший байт префикса S от CAN-LOG"
        },
        new TrackerTag
        {
            Id = 32, Name = "can8_bitr4", DataType = TagDataTypeEnum.Byte,
            Description = @"CAN8BITR4
или третий байт префикса P от CAN-LOG"
        },
        new TrackerTag
        {
            Id = 33, Name = "can8_bitr5", DataType = TagDataTypeEnum.Byte,
            Description = @"CAN8BITR5
или второй байт префикса P от CAN-LOG"
        },
        new TrackerTag
        {
            Id = Can32BitR0Id, Name = "can32_bitr0", DataType = TagDataTypeEnum.Integer,
            Description = @"В зависимости от настроек один из вариантов:
CAN32BITR0
полное время работы двигателя, ч"
        },
        new TrackerTag
        {
            Id = 35, Name = "port_4", DataType = TagDataTypeEnum.Integer,
            Description = @"Значение на входе 4.
В зависимости от настроек один из вариантов:
1. напряжение, мВ
2. число импульсов
3. частота, Гц"
        },
        new TrackerTag
        {
            Id = 36, Name = "port_5", DataType = TagDataTypeEnum.Integer,
            Description = @"Значение на входе 5.
В зависимости от настроек один из вариантов:
1. напряжение, мВ
2. число импульсов
3. частота, Гц"
        },
        new TrackerTag
        {
            Id = 37, Name = "i_button_1", DataType = TagDataTypeEnum.Integer,
            Description = @"Идентификационный номер первого ключа iButton"
        },
        new TrackerTag
        {
            Id = 38, Name = "i_button_2", DataType = TagDataTypeEnum.Integer,
            Description = @"Идентификационный номер второго ключа iButton"
        },
        new TrackerTag
        {
            Id = 39, Name = "i_button_keys", DataType = TagDataTypeEnum.Byte,
            Description = @"Состояние ключей iButton, идентификаторы которых заданы командой iButtons.
Каждый бит соответствует одному ключу.
Например, получено: 05 или 00000101 в двоичном виде. Это значит, что подсоединены первый и третий ключи."
        },
        new TrackerTag
        {
            Id = 40, Name = "expanded_terminal_status", DataType = TagDataTypeEnum.Bits,
            Description = @"0 – состояние подключения к основному серверу. 1- подключен, 0 – нет.
1 – статус GPRS сессии. 1- установлена, 0 – нет.
2 – признак глушения GSM. 1- обнаружено глушение, 0 – нет.
3 – состояние подключения к дополнительному серверу. 1 – подключен, 0 – нет.
4 – признак глушения GPS/GLONASS. 1- обнаружено глушение, 0 – нет.
5 – признак подключения к терминалу кабеля USB. 1 – подключен, 0 – не подключен.
6 – признак наличия SD карты в терминале. 1 – присутствует, 0 – отсутствует."
        },
        new TrackerTag
        {
            Id = MileageId, Name = "mileage", DataType = TagDataTypeEnum.Integer,
            Description = @"Общий пробег по данным GPS/ГЛОНАСС-модулей, м."
        },
    };
}
