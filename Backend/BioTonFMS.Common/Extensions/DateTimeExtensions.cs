namespace BioTonFMS.Common.Extensions;

public static class DateTimeExtensions
{
    public static DateTime ToToday(this DateTime dateTime)
    {
        var now = DateTime.UtcNow;
        return now.Add(-now.TimeOfDay).Add(dateTime.TimeOfDay).ToUniversalTime();
    }
}
