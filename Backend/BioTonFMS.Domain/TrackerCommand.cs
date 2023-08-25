using System.ComponentModel.DataAnnotations;
using BioTonFMS.Infrastructure.Models;

namespace BioTonFMS.Domain;

/// <summary>
/// Команда отправленная на трекер
/// </summary>
public class TrackerCommand : EntityBase, IAggregateRoot
{
    ///<summary>
    /// Id трекера на который отправлена команда
    ///</summary>
    [Required]
    public int TrackerId { get; set; }
    
    ///<summary>
    /// Ссылка на трекер
    ///</summary>
    public Tracker? Tracker { get; set; }
     
    ///<summary>
    /// Дата и время, когда команда была отправлена на трекер
    ///</summary>
    public DateTime SentDateTime { get; set; }

    ///<summary>
    /// Текст команды (вместе с параметрами) отправленный на трекер
    ///</summary>
    [MaxLength(100)]
    public string CommandText { get; set; } = "";

    ///<summary>
    /// Текст ответа на команду
    ///</summary>
    [MaxLength(200)]
    public string ResponseText { get; set; } = "";

    ///<summary>
    /// Бинарная информация, которая может прилагаться к ответу
    ///</summary>
    [MaxLength(200)]
    public byte[]? BinaryResponse { get; set; } = Array.Empty<byte>();

    /// <summary>
    /// Время получения ответа на команду
    /// </summary>
    public DateTime? ResponseDateTime { get; set; }
}