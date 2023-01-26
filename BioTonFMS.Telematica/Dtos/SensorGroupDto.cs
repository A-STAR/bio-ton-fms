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
    /// Описание группы датчиков
    /// </summary>
    public string Description { get; set; } = "";    
}
