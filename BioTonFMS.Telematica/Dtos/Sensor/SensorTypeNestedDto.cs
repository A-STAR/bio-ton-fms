using BioTonFMS.Domain;

namespace BioTonFMS.Telematica.Dtos;

/// <summary>
/// Тип датчиков
/// </summary>
public class SensorTypeNestedDto
{
    /// <summary>
    /// Id типа сенсоров
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Название типа датчика
    /// </summary>
    public string Name { get; set; } = "";
        
    /// <summary>
    /// Описание типа датчика
    /// </summary>
    public string Description { get; set; } = "";

    /// <summary>
    /// Идентификатор группы датчиков, к которой принадлежат датчики данного типа.
    /// </summary>
    public int SensorGroupId { get; set; }

    /// <summary>
    /// Идентификатор типа данных датчиков данного типа. Если не указан, то датчики могут иметь любые типы.
    /// </summary>
    public SensorDataTypeEnum? DataType { get; set; }

    /// <summary>
    /// Идентификатор единицы измерения для датчиков данного типа. Если не указан, то датчики могут иметь любые единицы измерения.
    /// </summary>
    public int? UnitId { get; set; }
}
