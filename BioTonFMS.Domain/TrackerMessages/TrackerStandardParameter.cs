namespace BioTonFMS.Domain.TrackerMessages;

public class TrackerStandardParameter
{
    public string Name { get; set; }
    public string ParamName { get; set; }
    public DateTime? LastValueDateTime { get; set; }
    public double? LastValueDecimal { get; set; }
}