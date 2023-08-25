namespace BioTonFMS.Common.Settings;

public class TrackerOptions
{
    /// <summary>
    /// Время валидности адреса трекера
    /// </summary>
    public int TrackerAddressValidMinutes { get; set; } = 60;

    /// <summary>
    /// Таймаут получения ответа для команды
    /// </summary>
    public int CommandTimeoutSec { get; set; } = 120;
}