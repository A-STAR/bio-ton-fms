namespace BioTonFMS.Telematica.Dtos.Monitoring;

public class TrackPointInfo
{
    /// <summary>
    /// Идентификатор сообщения
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    ///Дата и время получения сообщения
    /// </summary>
    public int Time { get; set; }

    /// <summary>
    ///Широта точки трека
    /// </summary>
    public int Latitude { get; set; }

    /// <summary>
    ///Долгота точки трека
    /// </summary>
    public int Longitude { get; set; }

    /// <summary>
    ///Скорость в точке (может быть пустым)
    /// </summary>
    public int Speed { get; set; }

    /// <summary>
    ///высота в точке (может быть пустым)
    /// </summary>
    public int Altitude { get; set; }
}