using BioTonFMS.Infrastructure.EF.Models.Filters;

namespace BioTonFMS.Infrastructure.EF.Repositories.Models.Filters;

public class ParametersHistoryFilter : PageableFilter
{
    public int ExternalId { get; set; }
}