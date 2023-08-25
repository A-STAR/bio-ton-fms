namespace BioTonFMS.TrackerProtocolSpecific.CommandCodecs;

public class CommandResponseInfo
{
    /// <summary>
    /// ExternalId трекера 
    /// </summary>
    public int ExternalId { get; set; }

    /// <summary>
    /// IMEI трекера
    /// </summary>
    public string Imei { get; set; } = string.Empty;

    /// <summary>
    /// Id записи репозитория команд
    /// </summary>
    public int CommandId { get; set; }

    /// <summary>
    /// Текст ответа
    /// </summary>
    public string ResponseText { get; set; } = string.Empty;

    /// <summary>
    /// Массив байт с данными пакета ответа
    /// </summary>
    public byte[]? ResponseBinary { get; set; }
}
