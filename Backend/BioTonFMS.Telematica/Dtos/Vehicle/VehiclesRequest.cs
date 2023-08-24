using BioTonFMS.Domain;
using BioTonFMS.Infrastructure.EF.Models;
using BioTonFMS.Infrastructure.EF.Repositories.Models;

namespace BioTonFMS.Telematica.Dtos.Vehicle
{
    /// <summary>
    /// Параметры выборки списка машин
    /// </summary>
    public class VehiclesRequest : RequestWithPaging
    {
        /// <summary>
        /// Наименование машины
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Тип машины
        /// </summary>
        public VehicleTypeEnum? Type { get; set; }

        /// <summary>
        /// Id группы машины
        /// </summary>
        public int? GroupId { get; set; }

        /// <summary>
        /// Подтип машины
        /// </summary>
        public VehicleSubTypeEnum? SubType { get; set; }

        /// <summary>
        /// Id типа топлива
        /// </summary>
        public int? FuelTypeId { get; set; }

        /// <summary>
        /// Регистрационный номер
        /// </summary>
        public string? RegNum { get; set; }

        /// <summary>
        /// Сортировка по полю
        /// </summary>
        public VehicleSortBy? SortBy { get; set; }

        /// <summary>
        /// Направление сортировки
        /// </summary>
        public SortDirection? SortDirection { get; set; }
    }
}
