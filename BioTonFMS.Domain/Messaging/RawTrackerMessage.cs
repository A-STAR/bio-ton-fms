namespace BioTonFMS.Domain.Messaging;

public class RawTrackerMessage
{
    /// <summary>
    /// тип протокола
    /// </summary>
    public TrackerTypeEnum TrackerType { get; set; }

    /// <summary>
    /// сообщение от трекера
    /// </summary>
    public byte[] RawMessage { get; set; } = null!;

    /// <summary>
    /// Идентификатор пакета данных трекера
    /// </summary>
    public Guid PackageUID { get; set; }
}