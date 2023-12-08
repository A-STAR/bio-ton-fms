namespace BioTonFMS.Domain.MessagesView;

public class ViewMessageStatisticsDto
{
    /// <summary>
    /// Количество отобранных сообщений
    /// </summary>
    public int NumberOfMessages { get; set; }

    /// <summary>
    /// Время в секундах между первым и последним сообщением от объекта
    /// </summary>
    public int TotalTime { get; set; }
    
    /// <summary>
    /// Расчетный пробег, пройденный объектом, за период в рамках отобранных сообщений
    /// В значении ставится прочерк при выборе типа сообщения «Отправка команд»
    /// </summary>
    public int Distance { get; set; }

    /// <summary>
    /// Данные пробега из последнего сообщения
    /// </summary>
    public int Mileage { get; set; }

    /// <summary>
    /// Среднюю скорость объекта
    /// </summary>
    public double AverageSpeed { get; set; }

    /// <summary>
    /// Максимальную скорость объекта из выбранных сообщений
    /// В значении ставится прочерк при выборе типа сообщения «Отправленные команды»
    /// </summary>
    public double MaxSpeed { get; set; }
}