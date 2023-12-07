using BioTonFMS.Domain.TrackerMessages;

namespace BioTonFMS.Domain.MessagesView;

public class DataMessageDto
{
    /// <summary>
    /// Порядковый номер сообщения в выборке
    /// </summary>
    public int Num { get; set; }

    /// <summary>
    /// Идентификатор сообщения
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Дата и время регистрации сообщения на сервере (в UTC)
    /// </summary>
    public DateTime ServerDateTime { get; set; }

    /// <summary>
    ///  Дата и время регистрации сообщения на трекере (в UTC)
    /// </summary>
    public DateTime? TrackerDateTime { get; set; }

    /// <summary>
    ///  Скорость движения объекта из сообщения, если в сообщении нет информации о скорости, то null
    /// </summary>
    public double? Speed { get; set; }

    /// <summary>
    ///  Широта в сообщении
    /// </summary>
    public double? Latitude { get; set; }

    /// <summary>
    ///  Долгота в сообщении
    /// </summary>
    public double? Longitude { get; set; }

    /// <summary>
    ///  Число спутников в сообщении
    /// </summary>
    public int? SatNumber { get; set; }

    /// <summary>
    ///  Высота в сообщении
    /// </summary>
    public double? Altitude { get; set; }
}

public class TrackerDataMessageDto : DataMessageDto
{
    /// <summary>
    /// Параметры трекера из сообщения
    /// </summary>
    public TrackerParameter[] Parameters { get; set; }
}

public class SensorDataMessageDto : DataMessageDto
{
    /// <summary>
    /// Значения датчиков, рассчитанные для сообщения
    /// </summary>
    public TrackerSensorDto[] Sensors { get; set; }
}