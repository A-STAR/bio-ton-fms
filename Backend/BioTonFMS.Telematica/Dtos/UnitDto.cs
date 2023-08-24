namespace BioTonFMS.Telematica.Dtos;

/// <summary>
/// Единица измерения
/// </summary>
public class UnitDto
{
    /// <summary>
    /// Id единицы измерения
    /// </summary>
    public int Id { get; set; }    
    
    /// <summary>
    /// Название единицы измерения
    /// </summary>
    public string Name { get; set; } = "";

    /// <summary>
    /// Сокращенное название единицы измерения, которое пишется после значения
    /// </summary>
    public string Abbreviated { get; set; } = "";
}
