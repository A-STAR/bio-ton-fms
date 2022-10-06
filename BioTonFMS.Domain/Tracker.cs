﻿using BioTonFMS.Infrastructure.Models;
using System.ComponentModel.DataAnnotations;

namespace BioTonFMS.Domain
{
    public class Tracker : EntityBase, IAggregateRoot
    {
        /// <summary>
        /// идентификатор трекера во внешней системе
        /// </summary>
        [Required]
        public int ExternalId { get; set; }

        /// <summary>
        /// Название трекера
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = "";

        /// <summary>
        /// Номер sim – карты
        /// </summary>
        [Required]
        [MaxLength(12)]
        public string SimNumber { get; set; } = "";

        /// <summary>
        /// Тип устройства
        /// </summary>
        [Required]
        public TrackerTypeEnum TrackerType { get; set; }

        /// <summary>
        /// Дата и время начала действия трекера на данной машине
        /// </summary>
        [Required]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Описание
        /// </summary>
        [MaxLength(500)]
        public string Description { get; set; } = "";

        /// <summary>
        /// IMEI трекера
        /// </summary>
        [Required]
        [MaxLength(15)]
        public string Imei { get; set; } = "";

        public virtual IEnumerable<Device> Devices { get; set; }
    }
}