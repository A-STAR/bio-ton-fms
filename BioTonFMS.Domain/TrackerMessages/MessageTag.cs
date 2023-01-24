using System.Collections;
using System.ComponentModel.DataAnnotations;
using BioTonFMS.Infrastructure.Models;

namespace BioTonFMS.Domain.TrackerMessages;

public abstract class MessageTag : EntityBase
{
    /// <summary>
    /// Уникальный идентификатор записи
    /// </summary>
    public new long Id { get; set; }

    /// <summary>
    /// Идентификатор сообщения
    /// </summary>
    public long TrackerMessageId { get; set; }

    /// <summary>
    /// Ссылка на сообщение
    /// </summary>
    public TrackerMessage TrackerMessage { get; set; } = null!;  

    /// <summary>
    /// Идентификатор тега
    /// </summary>
    public int TrackerTagId { get; set; }
}

public class MessageTagInteger : MessageTag
{
    /// <summary>
    /// Целочисленное значение тега
    /// </summary>
    public int Value { get; set; }
}

public class MessageTagDouble : MessageTag
{
    /// <summary>
    /// Вещественное значение тега
    /// </summary>
    public double Value { get; set; }
}

public class MessageTagBoolean : MessageTag
{
    /// <summary>
    /// Логическое значение тега
    /// </summary>
    public bool Value { get; set; }
}

public class MessageTagString : MessageTag
{
    /// <summary>
    /// Строковое значение тега
    /// </summary>
    [MaxLength(100)]
    public string Value { get; set; } = string.Empty;
}

public class MessageTagDateTime : MessageTag
{
    /// <summary>
    /// Значение тега дата и время
    /// </summary>
    public DateTime Value { get; set; }
}

public class MessageTagBits : MessageTag
{
    /// <summary>
    /// Значение тега набор битов
    /// </summary>
    [MaxLength(32)]
    public BitArray Value { get; set; } = null!;
}

public class MessageTagByte : MessageTag
{
    /// <summary>
    /// Целочисленное значение тега длиной в 1 байт
    /// </summary>
    public byte Value { get; set; }
}