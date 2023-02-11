using BioTonFMS.Infrastructure.Extensions;
using KeyValuePair = BioTonFMS.Infrastructure.Extensions.KeyValuePair;

namespace BioTonFMS.Telematica.Dtos.Tracker;

/// <summary>
/// Модель трекера
/// </summary>
public class TrackerDto
{
    /// <summary>
    /// Id трекера
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Идентификатор трекера во внешней системе
    /// </summary>
    public int ExternalId { get; set; }

    /// <summary>
    /// Название трекера
    /// </summary>
    public string Name { get; set; } = "";

    /// <summary>
    /// Номер sim – карты
    /// </summary>
    public string SimNumber { get; set; } = "";

    /// <summary>
    /// IMEI трекера
    /// </summary>
    public string Imei { get; set; } = "";

    /// <summary>
    /// Тип устройства
    /// </summary>
    public KeyValuePair TrackerType { get; set; }

    /// <summary>
    /// Машина
    /// </summary>
    public ForeignKeyValue<int, string>? Vehicle { get; set; }

    /// <summary>
    /// Дата и время начала действия трекера на данной машине
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// Описание
    /// </summary>
    public string Description { get; set; } = "";
}