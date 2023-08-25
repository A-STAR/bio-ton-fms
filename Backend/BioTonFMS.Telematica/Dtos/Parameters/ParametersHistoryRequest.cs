using System.ComponentModel.DataAnnotations;

namespace BioTonFMS.Telematica.Dtos.Parameters;

public class ParametersHistoryRequest : RequestWithPaging
{
    /// <summary>
    /// Идентификатор трекера для которого возвращается история
    /// </summary>
    [Required]
    public int TrackerId { get; set; }
}