using System.Net;

namespace BioTonFMS.Domain.Messaging;

public class RawTrackerMessage
{
    /// <summary>
    /// Тип протокола
    /// </summary>
    public TrackerTypeEnum TrackerType { get; set; }

    /// <summary>
    /// Сообщение от трекера
    /// </summary>
    public byte[] RawMessage { get; set; } = null!;

    /// <summary>
    /// Идентификатор пакета данных трекера
    /// </summary>
    public Guid PackageUID { get; set; }

    /// <summary>
    /// IP адрес трекера
    /// </summary>
    public string IpAddress { get; set; } = "";
    
    /// <summary>
    /// Порт трекера
    /// </summary>
    public int Port { get; set; }
}