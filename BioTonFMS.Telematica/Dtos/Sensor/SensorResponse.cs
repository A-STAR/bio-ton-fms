namespace BioTonFMS.Telematica.Dtos
{
    /// <summary>
    /// Список датчиков с информацией о постраничном выводе
    /// </summary>
    public class SensorsResponse
    {
        /// <summary>
        /// Массив датчиков
        /// </summary>
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        // ReSharper disable once PropertyCanBeMadeInitOnly.Global
        public SensorDto[] Sensors { get; set; } = null!;

        /// <summary>
        /// Параметр постраничного вывода
        /// </summary>
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        // ReSharper disable once PropertyCanBeMadeInitOnly.Global
        public Pagination Pagination { get; set; } = null!;
    }
}
