using BioTonFMS.Domain;

namespace BioTonFMS.Infrastructure.EF.Repositories.TrackerTags;

public static class TagsSeed
{
    public static readonly IEnumerable<ProtocolTag> ProtocolTags = new[]
    {
        new ProtocolTag
        {
            Id = 1,
            TagId = 1,
            ProtocolTagCode = 0x01,
            Size = 1,
            TrackerType = TrackerTypeEnum.GalileoSkyV50
        },
        new ProtocolTag
        {
            Id = 2,
            TagId = 2,
            ProtocolTagCode = 0x02,
            Size = 1,
            TrackerType = TrackerTypeEnum.GalileoSkyV50
        },
        new ProtocolTag
        {
            Id = 3,
            TagId = 3,
            ProtocolTagCode = 0x03,
            Size = 15,
            TrackerType = TrackerTypeEnum.GalileoSkyV50
        },
        new ProtocolTag
        {
            Id = 4,
            TagId = 4,
            ProtocolTagCode = 0x04,
            Size = 2,
            TrackerType = TrackerTypeEnum.GalileoSkyV50
        },
        new ProtocolTag
        {
            Id = 5,
            TagId = 5,
            ProtocolTagCode = 0x10,
            Size = 2,
            TrackerType = TrackerTypeEnum.GalileoSkyV50
        },
        new ProtocolTag
        {
            Id = 6,
            TagId = 6,
            ProtocolTagCode = 0x20,
            Size = 4,
            TrackerType = TrackerTypeEnum.GalileoSkyV50
        },
        new ProtocolTag
        {
            Id = 7,
            TagId = 7,
            ProtocolTagCode = 0x30,
            Size = 9,
            TrackerType = TrackerTypeEnum.GalileoSkyV50
        },
        new ProtocolTag
        {
            Id = 8,
            TagId = 8,
            ProtocolTagCode = 0x33,
            Size = 4,
            TrackerType = TrackerTypeEnum.GalileoSkyV50
        },
        new ProtocolTag
        {
            Id = 9,
            TagId = 9,
            ProtocolTagCode = 0x34,
            Size = 2,
            TrackerType = TrackerTypeEnum.GalileoSkyV50
        },
        new ProtocolTag
        {
            Id = 10,
            TagId = 10,
            ProtocolTagCode = 0x35,
            Size = 1,
            TrackerType = TrackerTypeEnum.GalileoSkyV50
        },
        new ProtocolTag
        {
            Id = 11,
            TagId = 11,
            ProtocolTagCode = 0x40,
            Size = 2,
            TrackerType = TrackerTypeEnum.GalileoSkyV50
        },
        new ProtocolTag
        {
            Id = 12,
            TagId = 12,
            ProtocolTagCode = 0x41,
            Size = 2,
            TrackerType = TrackerTypeEnum.GalileoSkyV50
        },
        new ProtocolTag
        {
            Id = 13,
            TagId = 13,
            ProtocolTagCode = 0x42,
            Size = 2,
            TrackerType = TrackerTypeEnum.GalileoSkyV50
        },
        new ProtocolTag
        {
            Id = 14,
            TagId = 14,
            ProtocolTagCode = 0x43,
            Size = 1,
            TrackerType = TrackerTypeEnum.GalileoSkyV50
        },
        new ProtocolTag
        {
            Id = 15,
            TagId = 15,
            ProtocolTagCode = 0x45,
            Size = 2,
            TrackerType = TrackerTypeEnum.GalileoSkyV50
        },
        new ProtocolTag
        {
            Id = 16,
            TagId = 16,
            ProtocolTagCode = 0x46,
            Size = 2,
            TrackerType = TrackerTypeEnum.GalileoSkyV50
        },
        new ProtocolTag
        {
            Id = 17,
            TagId = 17,
            ProtocolTagCode = 0x50,
            Size = 2,
            TrackerType = TrackerTypeEnum.GalileoSkyV50
        },
        new ProtocolTag
        {
            Id = 18,
            TagId = 18,
            ProtocolTagCode = 0x51,
            Size = 2,
            TrackerType = TrackerTypeEnum.GalileoSkyV50
        },
        new ProtocolTag
        {
            Id = 19,
            TagId = 19,
            ProtocolTagCode = 0x52,
            Size = 2,
            TrackerType = TrackerTypeEnum.GalileoSkyV50
        },
        new ProtocolTag
        {
            Id = 20,
            TagId = 20,
            ProtocolTagCode = 0x53,
            Size = 2,
            TrackerType = TrackerTypeEnum.GalileoSkyV50
        },
        new ProtocolTag
        {
            Id = 21,
            TagId = 21,
            ProtocolTagCode = 0x58,
            Size = 2,
            TrackerType = TrackerTypeEnum.GalileoSkyV50
        },
        new ProtocolTag
        {
            Id = 22,
            TagId = 22,
            ProtocolTagCode = 0x59,
            Size = 2,
            TrackerType = TrackerTypeEnum.GalileoSkyV50
        },
        new ProtocolTag
        {
            Id = 23,
            TagId = 23,
            ProtocolTagCode = 0x60,
            Size = 2,
            TrackerType = TrackerTypeEnum.GalileoSkyV50
        },
        new ProtocolTag
        {
            Id = 24,
            TagId = 24,
            ProtocolTagCode = 0x61,
            Size = 2,
            TrackerType = TrackerTypeEnum.GalileoSkyV50
        },
        new ProtocolTag
        {
            Id = 25,
            TagId = 25,
            ProtocolTagCode = 0xC0,
            Size = 4,
            TrackerType = TrackerTypeEnum.GalileoSkyV50
        },
        new ProtocolTag
        {
            Id = 26,
            TagId = 26,
            ProtocolTagCode = 0xC1,
            Size = 4,
            TrackerType = TrackerTypeEnum.GalileoSkyV50
        },
        new ProtocolTag
        {
            Id = 27,
            TagId = 27,
            ProtocolTagCode = 0xC2,
            Size = 4,
            TrackerType = TrackerTypeEnum.GalileoSkyV50
        },
        new ProtocolTag
        {
            Id = 28,
            TagId = 28,
            ProtocolTagCode = 0xC4,
            Size = 1,
            TrackerType = TrackerTypeEnum.GalileoSkyV50
        },
        new ProtocolTag
        {
            Id = 29,
            TagId = 29,
            ProtocolTagCode = 0xC5,
            Size = 1,
            TrackerType = TrackerTypeEnum.GalileoSkyV50
        },
        new ProtocolTag
        {
            Id = 30,
            TagId = 30,
            ProtocolTagCode = 0xC6,
            Size = 1,
            TrackerType = TrackerTypeEnum.GalileoSkyV50
        },
        new ProtocolTag
        {
            Id = 31,
            TagId = 31,
            ProtocolTagCode = 0xC7,
            Size = 1,
            TrackerType = TrackerTypeEnum.GalileoSkyV50
        },
        new ProtocolTag
        {
            Id = 32,
            TagId = 32,
            ProtocolTagCode = 0xC8,
            Size = 1,
            TrackerType = TrackerTypeEnum.GalileoSkyV50
        },
        new ProtocolTag
        {
            Id = 33,
            TagId = 33,
            ProtocolTagCode = 0xC9,
            Size = 1,
            TrackerType = TrackerTypeEnum.GalileoSkyV50
        },
        new ProtocolTag
        {
            Id = 34,
            TagId = 34,
            ProtocolTagCode = 0xDB,
            Size = 4,
            TrackerType = TrackerTypeEnum.GalileoSkyV50
        },
        new ProtocolTag
        {
            Id = 35,
            TagId = 35,
            ProtocolTagCode = 0x54,
            Size = 2,
            TrackerType = TrackerTypeEnum.GalileoSkyV50
        },
        new ProtocolTag
        {
            Id = 36,
            TagId = 36,
            ProtocolTagCode = 0x55,
            Size = 2,
            TrackerType = TrackerTypeEnum.GalileoSkyV50
        },
        new ProtocolTag
        {
            Id = 37,
            TagId = 37,
            ProtocolTagCode = 0x90,
            Size = 4,
            TrackerType = TrackerTypeEnum.GalileoSkyV50
        },
        new ProtocolTag
        {
            Id = 38,
            TagId = 38,
            ProtocolTagCode = 0xD3,
            Size = 4,
            TrackerType = TrackerTypeEnum.GalileoSkyV50
        },
        new ProtocolTag
        {
            Id = 39,
            TagId = 39,
            ProtocolTagCode = 0xD5,
            Size = 1,
            TrackerType = TrackerTypeEnum.GalileoSkyV50
        },
        new ProtocolTag
        {
            Id = 40,
            TagId = 40,
            ProtocolTagCode = 0x48,
            Size = 2,
            TrackerType = TrackerTypeEnum.GalileoSkyV50
        }
    };

    public static readonly IEnumerable<TrackerTag> TrackerTags = new[]
    {
        new TrackerTag
        {
            Id = 1,
            Name = "term_version",
            DataType = TagDataTypeEnum.Integer,
            StructType = null,
            Description = "Версия терминала"
        },
        new TrackerTag
        {
            Id = 2,
            Name = "soft",
            DataType = TagDataTypeEnum.Integer,
            StructType = null,
            Description = "Версия прошивки"
        },
        new TrackerTag
        {
            Id = 3,
            Name = "imei",
            DataType = TagDataTypeEnum.String,
            StructType = null,
            Description = "IMEI"
        },
        new TrackerTag
        {
            Id = 4,
            Name = "device_id",
            DataType = TagDataTypeEnum.Integer,
            StructType = null,
            Description = "Идентификатор устройства"
        },
        new TrackerTag
        {
            Id = 5,
            Name = "rec_sn",
            DataType = TagDataTypeEnum.Integer,
            StructType = null,
            Description = "Номер записи в архиве"
        },
        new TrackerTag
        {
            Id = 6,
            Name = "tracker_date",
            DataType = TagDataTypeEnum.DateTime,
            StructType = null,
            Description = "Дата и время регистрации на трекере"
        },
        new TrackerTag
        {
            Id = 7,
            Name = "coord_struct",
            DataType = TagDataTypeEnum.Struct,
            StructType = StructTypeEnum.Coordinates,
            Description =
                "Координаты в градусах, число спутников, признак корректности определения координат и источник координат."
        },
        new TrackerTag
        {
            Id = 8,
            Name = "speed_direction",
            DataType = TagDataTypeEnum.Struct,
            StructType = StructTypeEnum.SpeedAndDirection,
            Description = "Скорость в км/ч и направление в градусах"
        },
        new TrackerTag
        {
            Id = 9,
            Name = "altitude",
            DataType = TagDataTypeEnum.Integer,
            StructType = null,
            Description = "Высота, м"
        },
        new TrackerTag
        {
            Id = 10,
            Name = "hdop",
            DataType = TagDataTypeEnum.Byte,
            StructType = null,
            Description = @"Одно из значений:
1. HDOP, если источник координат ГЛОНАСС/GPS модуль
2. Погрешность в метрах, если источник базовые станции GSM-сети"
        },
        new TrackerTag
        {
            Id = 11,
            Name = "dev_status",
            DataType = TagDataTypeEnum.Integer,
            StructType = null,
            Description = "Статус устройства"
        },
        new TrackerTag
        {
            Id = 12,
            Name = "pwr_ext",
            DataType = TagDataTypeEnum.Integer,
            StructType = null,
            Description = "Напряжение питания, мВ"
        },
        new TrackerTag
        {
            Id = 13,
            Name = "pwr_int",
            DataType = TagDataTypeEnum.Integer,
            StructType = null,
            Description = "Напряжение на батарее"
        },
        new TrackerTag
        {
            Id = 14,
            Name = "temp_int",
            DataType = TagDataTypeEnum.Integer,
            StructType = null,
            Description = "Температура внутри терминала, °С"
        },
        new TrackerTag
        {
            Id = 15,
            Name = "out",
            DataType = TagDataTypeEnum.Bits,
            StructType = null,
            Description = "Статус выходов"
        },
        new TrackerTag
        {
            Id = 16,
            Name = "in",
            DataType = TagDataTypeEnum.Bits,
            StructType = null,
            Description = "Статус входов"
        },
        new TrackerTag
        {
            Id = 17,
            Name = "adc1",
            DataType = TagDataTypeEnum.Integer,
            StructType = null,
            Description = @"Значение на входе 0.
В зависимости от настроек один из вариантов:
1. Напряжение, Мв
2. число импульсов
3. частота, Гц"
        },
        new TrackerTag
        {
            Id = 18,
            Name = "adc2",
            DataType = TagDataTypeEnum.Integer,
            StructType = null,
            Description = @"Значение на входе 1.
В зависимости от настроек один из вариантов:
1. Напряжение, Мв
2. число импульсов
3. частота, Гц"
        },
        new TrackerTag
        {
            Id = 19,
            Name = "adc3",
            DataType = TagDataTypeEnum.Integer,
            StructType = null,
            Description = @"Значение на входе 2.
В зависимости от настроек один из вариантов:
1. Напряжение, Мв
2. число импульсов
3. частота, Гц"
        },
        new TrackerTag
        {
            Id = 20,
            Name = "adc4",
            DataType = TagDataTypeEnum.Integer,
            StructType = null,
            Description = @"Значение на входе 3.
В зависимости от настроек один из вариантов:
1. Напряжение, Мв
2. число импульсов
3. частота, Гц"
        },
        new TrackerTag
        {
            Id = 21,
            Name = "adc9",
            DataType = TagDataTypeEnum.Bits,
            StructType = null,
            Description = "RS232 0"
        },
        new TrackerTag
        {
            Id = 22,
            Name = "adc10",
            DataType = TagDataTypeEnum.Bits,
            StructType = null,
            Description = "RS232 1"
        },
        new TrackerTag
        {
            Id = 23,
            Name = "RS485[0]",
            DataType = TagDataTypeEnum.Integer,
            StructType = null,
            Description = "RS485[0]. ДУТ с адресом 0"
        },
        new TrackerTag
        {
            Id = 24,
            Name = "RS485[1]",
            DataType = TagDataTypeEnum.Integer,
            StructType = null,
            Description = "RS485[1]. ДУТ с адресом 1"
        },
        new TrackerTag
        {
            Id = 25,
            Name = "CAN_A0",
            DataType = TagDataTypeEnum.Integer,
            StructType = null,
            Description = @"Данные CAN-шины (CAN_A0) или CAN-LOG.
Топливо, израсходованное машиной с момента её создания, л"
        },
        new TrackerTag
        {
            Id = 26,
            Name = "CAN_A1",
            DataType = TagDataTypeEnum.Struct,
            StructType = StructTypeEnum.FuelLevel_TempCool_EngineSpeed,
            Description = @"Данные CAN-шины (CAN_A1) или CAN-LOG.
Уровень топлива, %;
температура охлаждающей жидкости, °C;
обороты двигателя, об/мин."
        },
        new TrackerTag
        {
            Id = 27,
            Name = "CAN_B0",
            DataType = TagDataTypeEnum.Integer,
            StructType = null,
            Description = @"Данные CAN-шины (CAN_B0) или CAN-LOG.
Пробег автомобиля, м."
        },
        new TrackerTag
        {
            Id = 28,
            Name = "CAN8BITR0",
            DataType = TagDataTypeEnum.Byte,
            StructType = null,
            Description = @"CAN8BITR0
или скорость транспортного средства, передаваемая с CAN-LOG’а, км/ч"
        },
        new TrackerTag
        {
            Id = 29,
            Name = "CAN8BITR1",
            DataType = TagDataTypeEnum.Byte,
            StructType = null,
            Description = @"CAN8BITR1
или второй байт префикса S от CAN-LOG"
        },
        new TrackerTag
        {
            Id = 30,
            Name = "CAN8BITR2",
            DataType = TagDataTypeEnum.Byte,
            StructType = null,
            Description = @"CAN8BITR2
или первый байт префикса S от CAN-LOG"
        },
        new TrackerTag
        {
            Id = 31,
            Name = "CAN8BITR3",
            DataType = TagDataTypeEnum.Byte,
            StructType = null,
            Description = @"CAN8BITR3
или младший байт префикса S от CAN-LOG"
        },
        new TrackerTag
        {
            Id = 32,
            Name = "CAN8BITR4",
            DataType = TagDataTypeEnum.Byte,
            StructType = null,
            Description = @"CAN8BITR4
или третий байт префикса P от CAN-LOG"
        },
        new TrackerTag
        {
            Id = 33,
            Name = "CAN8BITR5",
            DataType = TagDataTypeEnum.Byte,
            StructType = null,
            Description = @"CAN8BITR5
или второй байт префикса P от CAN-LOG"
        },
        new TrackerTag
        {
            Id = 34,
            Name = "CAN32BITR0",
            DataType = TagDataTypeEnum.Integer,
            StructType = null,
            Description = @"В зависимости от настроек один из вариантов:
CAN32BITR0
полное время работы двигателя, ч"
        },
        new TrackerTag
        {
            Id = 35,
            Name = "Port 4",
            DataType = TagDataTypeEnum.Integer,
            StructType = null,
            Description = @"Значение на входе 4.
В зависимости от настроек один из вариантов:
1. напряжение, мВ
2. число импульсов
3. частота, Гц"
        },
        new TrackerTag
        {
            Id = 36,
            Name = "Port 5",
            DataType = TagDataTypeEnum.Integer,
            StructType = null,
            Description = @"Значение на входе 5.
В зависимости от настроек один из вариантов:
1. напряжение, мВ
2. число импульсов
3. частота, Гц"
        },
        new TrackerTag
        {
            Id = 37,
            Name = "iButton 1",
            DataType = TagDataTypeEnum.Integer,
            StructType = null,
            Description = @"Идентификационный номер первого ключа iButton"
        },
        new TrackerTag
        {
            Id = 38,
            Name = "iButton 2",
            DataType = TagDataTypeEnum.Integer,
            StructType = null,
            Description = @"Идентификационный номер второго ключа iButton"
        },
        new TrackerTag
        {
            Id = 39,
            Name = "iButton Keys",
            DataType = TagDataTypeEnum.Byte,
            StructType = null,
            Description = @"Состояние ключей iButton, идентификаторы которых заданы командой iButtons.
Каждый бит соответствует одному ключу.
Например, получено: 05 или 00000101 в двоичном виде. Это значит, что подсоединены первый и третий ключи."
        },
        new TrackerTag
        {
            Id = 40,
            Name = "expanded_terminal_status",
            DataType = TagDataTypeEnum.Bits,
            StructType = null,
            Description = @"0 – состояние подключения к основному серверу. 1- подключен, 0 – нет.
1 – статус GPRS сессии. 1- установлена, 0 – нет.
2 – признак глушения GSM. 1- обнаружено глушение, 0 – нет.
3 – состояние подключения к дополнительному серверу. 1 – подключен, 0 – нет.
4 – признак глушения GPS/GLONASS. 1- обнаружено глушение, 0 – нет.
5 – признак подключения к терминалу кабеля USB. 1 – подключен, 0 – не подключен.
6 – признак наличия SD карты в терминале. 1 – присутствует, 0 – отсутствует."
        }
    };
}