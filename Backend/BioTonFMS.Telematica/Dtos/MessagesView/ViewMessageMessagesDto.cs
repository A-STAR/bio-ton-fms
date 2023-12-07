using BioTonFMS.Domain.MessagesView;

namespace BioTonFMS.Telematica.Dtos.MessagesView;

public class ViewMessageMessagesDto
{
    /// <summary>
    /// Набор сообщений с данными полученными от трекера
    /// Заполнен если в запросе ViewMessageType == DataMessage и ParameterType == TrackerData
    /// </summary>
    public TrackerDataMessageDto[] TrackerDataMessages { get; set; } 
    
    /// <summary>
    /// Набор сообщений со значениями датчиков рассчитанными по сообщениям полученным от трекера
    /// Заполнен если в запросе ViewMessageType == DataMessage и ParameterType == SensorData
    /// </summary>
    public SensorDataMessageDto[] SensorDataMessages { get; set; }
    
    /// <summary>
    /// Набор сообщений с информацией о посланных командах
    /// </summary>
    public CommandMessageDto[] CommandMessages { get; set; }
    
    /// <summary>
    /// Параметр постраничного вывода
    /// </summary>
    public Pagination Pagination { get; set; }
}