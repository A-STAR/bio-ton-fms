namespace BioTonFMS.Domain.Messaging;

public class TrackerCommandMessage
{
    /// <summary>
    /// ExternalId трекера 
    /// </summary>
    //public int ExternalId { get; set; }

    /// <summary>
    /// IMEI трекера
    /// </summary>
    //public string Imei { get; set; }

    /// <summary>
    /// IP адрес трекера
    /// </summary>
    public string IpAddress { get; set; }

    /// <summary>
    /// Порт трекера
    /// </summary>
    public int Port { get; set; }

    /// <summary>
    /// Id записи репозитория команд
    /// </summary>
    public int CommandId { get; set; }

    /// <summary>
    /// Массив байт содержащий закодированный пакет данных с командой, которую нужно отправить на трекер
    /// </summary>
    public byte[] EncodedCommand { get; set; }

    /// <summary>
    /// Время отправки команды (UTC)
    /// </summary>
    public DateTime SentDateTime { get; set; }
    
}