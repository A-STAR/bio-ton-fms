namespace BioTonFMS.Domain.TrackerMessages;

/// <summary>
/// Запись в истории значений параметров трекера
/// </summary>
public class ParametersHistoryRecord
{
    /// <summary>
    /// Дата и время получения сообщения
    /// </summary>
    public DateTime Time { get; set; }
    
    /// <summary>
    /// Широта
    /// </summary>
    public double? Latitude { get; set; }
    
    /// <summary>
    /// Долгота
    /// </summary>
    public double? Longitude { get; set; }
    
    /// <summary>
    /// Высота
    /// </summary>
    public double? Altitude { get; set; }
    
    /// <summary>
    /// Скорость
    /// </summary>
    public double? Speed { get; set; }

    /// <summary>
    /// Теги параметров, не включая теги сенсоров, объединённые в одну строку 
    /// </summary>
    public string Parameters { get; set; } = "";
}