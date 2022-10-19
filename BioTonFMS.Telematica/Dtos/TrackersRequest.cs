﻿using BioTonFMS.Domain;
using BioTonFMS.Infrastructure.EF.Models;
using BioTonFMS.Infrastructure.EF.Repositories.Models;
using System.ComponentModel.DataAnnotations;

namespace BioTonFMS.Telematica.Dtos
{
    /// <summary>
    /// Параметры выборки списка трекеров
    /// </summary>
    public class TrackersRequest
    {
        /// <summary>
        /// Внешний идентификатор
        /// </summary>
        public int? ExternalId { get; set; }

        /// <summary>
        /// Номер сим карты
        /// </summary>
        public string? SimNumber { get; set; }

        /// <summary>
        /// Тип устройства
        /// </summary>
        public TrackerTypeEnum? Type { get; set; }

        /// <summary>
        /// Сортировка по полю
        /// </summary>
        public TrackerSortBy? SortBy { get; set; }

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
