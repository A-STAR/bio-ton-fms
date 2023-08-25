namespace BioTonFMS.Infrastructure.EF.Repositories.Models
{
    /// <summary>
    /// Перечисление сортировки трекеров по полю
    /// </summary>
    public enum TrackerSortBy
    {
        /// <summary>
        /// Название трекера
        /// </summary>
        Name,
        /// <summary>
        /// Идентификатор трекера во внешней системе
        /// </summary>
        ExternalId,
        /// <summary>
        /// Тип устройства
        /// </summary>
        Type,
        /// <summary>
        /// Номер sim – карты
        /// </summary>
        SimNumber,
        /// <summary>
        /// Машина
        /// </summary>
        Vehicle,
        /// <summary>
        /// Дата и время начала действия трекера на данной машине
        /// </summary>
        StartDate
    }
}
