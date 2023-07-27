using BioTonFMS.Domain.Monitoring;

namespace BioTonFMS.Domain.TrackerMessages;

public class VehicleStatus
{
    /// <summary>
    /// Идентификатор трекера во внешней системе
    /// </summary>
    public int TrackerExternalId { get; set; }

    /// <summary>
    /// Состояние движения
    /// </summary>
    public MovementStatusEnum MovementStatus { get; set; }

    /// <summary>
    /// Состояние соединения
    /// </summary>
    public ConnectionStatusEnum ConnectionStatus { get; set; }

    /// <summary>
    /// Время последнего сообщения трекера
    /// </summary>
    public DateTime? LastMessageTime { get; set; }

    /// <summary>
    /// Количество спутников
    /// </summary>
    public int? NumberOfSatellites { get; set; }
}