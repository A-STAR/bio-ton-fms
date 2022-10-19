﻿using BioTonFMS.Domain;

namespace BioTonFMS.Infrastructure.EF.Models
{
    public class UpdateTrackerModel
    {
        /// <summary>
        /// Идентификатор трекера во внешней системе
        /// </summary>
        public int ExternalId { get; set; }

        /// <summary>
        /// Номер sim – карты
        /// </summary>
        public string SimNumber { get; set; }

        /// <summary>
        /// Тип устройства
        /// </summary>
        public TrackerTypeEnum TrackerType { get; set; }

        /// <summary>
        /// Дата и время начала действия трекера на данной машине
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Описание
        /// </summary>
        public string Description { get; set; }
    }
}
