﻿using BioTonFMS.Domain;

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
        public string Name { get; set; }

        /// <summary>
        /// Тип машины
        /// </summary>
        public VehicleTypeEnum Type { get; set; }

        /// <summary>
        /// Id группы машины
        /// </summary>
        public int VehicleGroupId { get; set; }

        /// <summary>
        /// Бренд производителя машины
        /// </summary>
        public string Make { get; set; } = "";

        /// <summary>
        /// Модель машины
        /// </summary>
        public string Model { get; set; } = "";

        /// <summary>
        /// Подтип машины
        /// </summary>
        public VehicleSubTypeEnum SubType { get; set; }

        /// <summary>
        /// Id типа топлива
        /// </summary>
        public int FuelTypeId { get; set; }

        /// <summary>
        /// Год выпуска
        /// </summary>
        public int ManufacturingYear { get; set; }

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
    }
}