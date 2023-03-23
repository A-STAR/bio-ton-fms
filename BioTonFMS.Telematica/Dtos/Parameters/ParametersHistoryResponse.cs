using BioTonFMS.Domain.TrackerMessages;

namespace BioTonFMS.Telematica.Dtos.Parameters;

/// <summary>
/// История параметров трекера с информацией о постраничном выводе
/// </summary>
public class ParametersHistoryResponse
{
    /// <summary>
    /// Массив записей параметров трекеров
    /// </summary>
    public ICollection<ParametersHistoryRecord> Parameters { get; set; } = null!;
    
    /// <summary>
    /// Параметр постраничного вывода
    /// </summary>
    public Pagination Pagination { get; set; } = null!;
}