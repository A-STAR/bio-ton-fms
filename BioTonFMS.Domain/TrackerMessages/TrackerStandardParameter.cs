using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace BioTonFMS.Domain.TrackerMessages;

public class TrackerStandardParameter
{
    /// <summary>
    /// Название параметра
    /// </summary>
    [Required]
    public string Name { get; set; } = string.Empty;
    /// <summary>
    /// Имя параметра
    /// </summary>
    [Required]
    public string ParamName { get; set; } = string.Empty;
    /// <summary>
    /// Последнее значение дата и время
    /// </summary>
    public DateTime? LastValueDateTime { get; set; }
    /// <summary>
    /// Последнее значение число
    /// </summary>
    public double? LastValueDecimal { get; set; }
}