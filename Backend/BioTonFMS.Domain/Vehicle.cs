using BioTonFMS.Infrastructure.Models;
using System.ComponentModel.DataAnnotations;

namespace BioTonFMS.Domain
{
    /// <summary>
    /// Машина
    /// </summary>
    public class Vehicle : EntityBase, IAggregateRoot
    {
        /// <summary>
        /// Наименование машины
        /// </summary>
        [Required]
        [MaxLength(100)]
        [StringLength(100)]
        public string Name { get; set; } = "";

        /// <summary>
        /// Тип машины
        /// </summary>
        [Required]
        public VehicleTypeEnum Type { get; set; }

        /// <summary>
        /// Id группы машин
        /// </summary>
        public int? VehicleGroupId { get; set; }

        /// <summary>
        /// Группа машин
        /// </summary>
        public VehicleGroup? VehicleGroup { get; set; } = null!;

        /// <summary>
        /// Бренд производителя машины
        /// </summary>
        [MaxLength(30)]
        [StringLength(30)]
        [Required]
        public string Make { get; set; } = "";

        /// <summary>
        /// Модель машины
        /// </summary>
        [MaxLength(30)]
        [StringLength(30)]
        [Required]
        public string Model { get; set; } = "";

        /// <summary>
        /// Подтип машины
        /// </summary>
        [Required]
        public VehicleSubTypeEnum VehicleSubType { get; set; }

        /// <summary>
        /// Id типа топлива
        /// </summary>
        public int FuelTypeId { get; set; }

        /// <summary>
        /// Тип топлива
        /// </summary>
        [Required]
        public FuelType FuelType { get; set; } = null!;

        /// <summary>
        /// Год выпуска
        /// </summary>
        public int? ManufacturingYear { get; set; }

        /// <summary>
        /// Регистрационный номер
        /// </summary>
        [MaxLength(15)]
        [StringLength(15)]
        public string RegistrationNumber { get; set; } = "";

        /// <summary>
        /// Инвентарный номер
        /// </summary>
        [MaxLength(30)]
        [StringLength(30)]
        public string InventoryNumber { get; set; } = "";

        /// <summary>
        /// Серийный номер
        /// </summary>
        [MaxLength(40)]
        [StringLength(40)]
        public string SerialNumber { get; set; } = "";

        /// <summary>
        /// Описание
        /// </summary>
        [MaxLength(500)]
        [StringLength(500)]
        public string Description { get; set; } = "";

        /// <summary>
        /// Id трекера, закреплённый за машиной
        /// </summary>
        public int? TrackerId { get; set; }

        /// <summary>
        /// Трекер, закреплённый за машиной
        /// </summary>
        public Tracker? Tracker { get; set; }
    }
}