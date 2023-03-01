namespace BioTonFMS.Domain.TrackerMessages;

public class TrackerStandardParameter
{
    /// <summary>
    /// Название параметра
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// Имя параметра
    /// </summary>
    public string ParamName { get; set; }
    /// <summary>
    /// Последнее значение дата и время
    /// </summary>
    public DateTime? LastValueDateTime { get; set; }
    /// <summary>
    /// Последнее значение число
    /// </summary>
    public double? LastValueDecimal { get; set; }
}