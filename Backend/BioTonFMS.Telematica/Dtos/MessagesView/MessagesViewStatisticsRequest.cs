namespace BioTonFMS.Telematica.Dtos.MessagesView;

public class MessagesViewStatisticsRequest
{
    /// <summary>
    /// Id машины для которой выбираются сообщения
    /// </summary>
    public int VehicleId { get; set; }
    
    /// <summary>
    /// Время в UTC начиная с которого выбираются сообщения
    /// </summary>
    public DateTime PeriodStart { get; set; }
    
    /// <summary>
    /// Время в UTC заканчивая которым выбираются сообщения
    /// </summary>
    public DateTime PeriodEnd { get; set; }
    
    /// <summary>
    /// Тип сообщений
    /// </summary>
    public ViewMessageTypeEnum ViewMessageType { get; set; }
    
    /// <summary>
    /// Тип данных, возвращаемых методом
    /// В случае ViewMessageType = CommandMessage этот входной параметр - пустой
    /// </summary>
    public ParameterTypeEnum? ParameterType { get; set; }
}