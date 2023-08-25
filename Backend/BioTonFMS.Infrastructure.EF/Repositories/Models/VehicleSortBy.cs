namespace BioTonFMS.Infrastructure.EF.Repositories.Models
{
    /// <summary>
    /// Перечисление сортировки машин по полю
    /// </summary>
    public enum VehicleSortBy
    {
        /// <summary>
        /// Наименование машины
        /// </summary>
        Name,
        /// <summary>
        /// Тип машины
        /// </summary>
        Type,
        /// <summary>
        /// Группа машины
        /// </summary>
        Group,
        /// <summary>
        /// Подтип машины
        /// </summary>
        SubType,
        /// <summary>
        /// Тип топлива
        /// </summary>
        FuelType
    }

}
