namespace BioTonFMS.Domain.TrackerMessages;

public class TrackerStandardParameters
{
    public DateTime? Time { get; set; }
    public double? Lat { get; set; }
    public double? Long { get; set; }
    public double? Alt { get; set; }
    public double? Speed { get; set; }

    public TrackerStandardParameter[] GetArray() =>
        new[]
        {
            new TrackerStandardParameter
            {
                Name = "Время",
                ParamName = "time",
                LastValueDateTime = Time
            },
            new TrackerStandardParameter
            {
                Name = "Широта",
                ParamName = "lat",
                LastValueDecimal = Lat
            },
            new TrackerStandardParameter
            {
                Name = "Долгота",
                ParamName = "long",
                LastValueDecimal = Long
            },
            new TrackerStandardParameter
            {
                Name = "Высота",
                ParamName = "alt",
                LastValueDecimal = Alt
            },
            new TrackerStandardParameter
            {
                Name = "Скорость",
                ParamName = "speed",
                LastValueDecimal = Speed
            }
        };
}