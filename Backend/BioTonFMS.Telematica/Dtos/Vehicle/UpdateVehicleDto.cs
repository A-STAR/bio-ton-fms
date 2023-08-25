using BioTonFMS.Domain;
using System.ComponentModel.DataAnnotations;

namespace BioTonFMS.Telematica.Dtos.Vehicle
{
    /// <summary>
    /// Модель обновления машины
    /// </summary>
    public class UpdateVehicleDto
    {
        /// <summary>
        /// Наименование машины
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Id трекера, закреплённый за машиной
        /// </summary>
        public int? TrackerId { get; set; }

        /// <summary>
        /// Тип машины
        /// </summary>
        [EnumDataType(typeof(VehicleTypeEnum))]
        public VehicleTypeEnum Type { get; set; }

        /// <summary>
        /// Id группы машин
        /// </summary>
        public int? VehicleGroupId { get; set; }

        /// <summary>
        /// Бренд производителя машины
        /// </summary>
        public string Make { get; set; } = string.Empty;

        /// <summary>
        /// Модель машины
        /// </summary>
        public string Model { get; set; } = string.Empty;

        /// <summary>
        /// Подтип машины
        /// </summary>
        [EnumDataType(typeof(VehicleSubTypeEnum))]
        public VehicleSubTypeEnum SubType { get; set; }

        /// <summary>
        /// Id типа топлива
        /// </summary>
        public int FuelTypeId { get; set; }

        /// <summary>
        /// Год выпуска
        /// </summary>
        public int? ManufacturingYear { get; set; }

        /// <summary>
        /// Регистрационный номер
        /// </summary>
        public string RegistrationNumber { get; set; } = string.Empty;

        /// <summary>
        /// Инвентарный номер
        /// </summary>
        public string InventoryNumber { get; set; } = string.Empty;

        /// <summary>
        /// Серийный номер
        /// </summary>
        public string SerialNumber { get; set; } = string.Empty;

        /// <summary>
        /// Описание
        /// </summary>
        public string Description { get; set; } = string.Empty;
    }
}
