// ReSharper disable UnusedMember.Global
#pragma warning disable CS8618

namespace BioTonFMS.Telematica.Dtos;

/// <summary>
/// Группа датчиков
/// </summary>
public class SensorGroupDto
{
    /// <summary>
    /// Id группы датчиков
    /// </summary>
    public int Id { get; set; }    
    
    /// <summary>
    /// Название группы датчиков
    /// </summary>
    public string Name { get; set; } = "";
    
    /// <summary>
    /// Типы датчиков данной группы
    /// </summary>
    public List<SensorTypeNestedDto> SensorTypes { get; set; }

    /// <summary>
    /// Описание группы датчиков
    /// </summary>
    public string Description { get; set; } = "";    
}
