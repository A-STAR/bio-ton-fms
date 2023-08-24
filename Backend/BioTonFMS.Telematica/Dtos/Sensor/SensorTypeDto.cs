using BioTonFMS.Domain;
using BioTonFMS.Infrastructure.Extensions;

namespace BioTonFMS.Telematica.Dtos;

/// <summary>
/// Тип датчиков
/// </summary>
public class SensorTypeDto
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
    /// Группа датчиков, к которой принадлежат датчики данного типа.
    /// </summary>
    public ForeignKeyValue<int, string> SensorGroup { get; set; }

    /// <summary>
    /// Идентификатор типа данных датчиков данного типа. Если не указан, то датчики могут иметь любые типы.
    /// </summary>
    public SensorDataTypeEnum? DataType { get; set; }

    /// <summary>
    /// Единица измерения для датчиков данного типа. Если не указан, то датчики могут иметь любые единицы измерения.
    /// </summary>
    public ForeignKeyValue<int, string>? Unit { get; set; }
}
