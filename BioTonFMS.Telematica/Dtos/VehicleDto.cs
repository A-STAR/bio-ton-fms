using System.ComponentModel.DataAnnotations;
using KeyValuePair = BioTonFMS.Infrastructure.Extensions.KeyValuePair;

namespace BioTonFMS.Telematica.Dtos
{
    /// <summary>
    /// Модель машины
    /// </summary>
    public class VehicleDto
    {
        /// <summary>
        /// Id машины
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Наименование машины
        /// </summary>
        public string Name { get; set; } = "";

        /// <summary>
        /// Тип машины (ключ-значение)
        /// </summary>
        public KeyValuePair Type { get; set; }

        /// <summary>
        /// Группа машины (ключ-значение)
        /// </summary>
        public KeyValuePair? VehicleGroup { get; set; }

        /// <summary>
        /// Бренд производителя машины
        /// </summary>
        public string Make { get; set; } = "";

        /// <summary>
        /// Модель машины
        /// </summary>
        public string Model { get; set; } = "";

        /// <summary>
        /// Подтип машины (ключ-значение)
        /// </summary>
        public KeyValuePair SubType { get; set; }

        /// <summary>
        /// Типа топлива (ключ-значение)
        /// </summary>
        public KeyValuePair FuelType { get; set; }

        /// <summary>
        /// Год выпуска
        /// </summary>
        public int? ManufacturingYear { get; set; }

        /// <summary>
        /// Регистрационный номер
        /// </summary>
        public string RegistrationNumber { get; set; } = "";

        /// <summary>
        /// Инвентарный номер
        /// </summary>
        public string InventoryNumber { get; set; } = "";

        /// <summary>
        /// Серийный номер
        /// </summary>
        public string SerialNumber { get; set; } = "";

        /// <summary>
        /// Описание
        /// </summary>
        public string Description { get; set; } = "";

        /// <summary>
        /// Трекер (ключ-значение)
        /// </summary>
        public KeyValuePair? Tracker { get; set; }
    }

    /// <summary>
    /// Трекер
    /// </summary>
    public class VehicleTrackerDto
    {
        /// <summary>
        /// Id трекера
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Наименование трекера
        /// </summary>
        [Required]
        public string Name { get; set; } = "";
    }

}
