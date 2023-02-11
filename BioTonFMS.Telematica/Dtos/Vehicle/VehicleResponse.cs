namespace BioTonFMS.Telematica.Dtos.Vehicle
{
    /// <summary>
    /// Список машин с информацией о постраничном выводе
    /// </summary>
    public class VehicleResponse
    {
        /// <summary>
        /// Массив машин
        /// </summary>
        public VehicleDto[] Vehicles { get; set; } = null!;

        /// <summary>
        /// Параметр постраничного вывода
        /// </summary>
        public Pagination Pagination { get; set; } = null!;
    }
}
