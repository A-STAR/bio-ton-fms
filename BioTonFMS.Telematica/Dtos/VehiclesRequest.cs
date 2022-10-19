using BioTonFMS.Domain;
using BioTonFMS.Infrastructure.EF.Models;
using BioTonFMS.Infrastructure.EF.Repositories.Models;
using System.ComponentModel.DataAnnotations;

namespace BioTonFMS.Telematica.Dtos
{
    /// <summary>
    /// Параметры выборки списка машин
    /// </summary>
    public class VehiclesRequest
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

        /// <summary>
        /// Номер страницы для постраничного вывода
        /// </summary>
        [Required]
        public int PageNum { get; set; }

        /// <summary>
        /// Размер страницы для постраничного вывода
        /// </summary>
        [Required]
        public int PageSize { get; set; }
    }
}
