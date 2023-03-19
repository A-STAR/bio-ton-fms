using System.Collections;
using System.ComponentModel.DataAnnotations;
using BioTonFMS.Infrastructure.Models;
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable PropertyCanBeMadeInitOnly.Global

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
    public int? TrackerTagId { get; set; }

    /// <summary>
    /// Идентификатор датчика
    /// </summary>
    public int? SensorId { get; set; }

    /// <summary>
    /// Дискриминатор
    /// </summary>
    public TagDataTypeEnum TagType { get; set; }

    /// <summary>
    /// Является ли значение значением взятым из прошлых сообщений
    /// </summary>
    [Required]
    public bool IsFallback { get; set; }

    public static MessageTag Create<TValue>(TValue value)
    {
        return value switch
        {
            int v => new MessageTagInteger()
            {
                Value = v
            },
            double v => new MessageTagDouble()
            {
                Value = v
            },
            bool v => new MessageTagBoolean()
            {
                Value = v
            },
            string v => new MessageTagString()
            {
                Value = v
            },
            DateTime v => new MessageTagDateTime()
            {
                Value = v
            },
            BitArray v => new MessageTagBits()
            {
                Value = v
            },
            byte v => new MessageTagByte()
            {
                Value = v
            },
            _ => throw new ArgumentException($"Type of value {value?.GetType()} is not supported!", nameof(value))
        };
    }
    public abstract object GetValue();
}
public class MessageTagInteger : MessageTag
{
    /// <summary>
    /// Целочисленное значение тега
    /// </summary>
    public int Value { get; set; }

    public override object GetValue()
    {
        return Value;
    }
}
public class MessageTagDouble : MessageTag
{
    /// <summary>
    /// Вещественное значение тега
    /// </summary>
    public double Value { get; set; }

    public override object GetValue()
    {
        return Value;
    }
}
public class MessageTagBoolean : MessageTag
{
    /// <summary>
    /// Логическое значение тега
    /// </summary>
    public bool Value { get; set; }

    public override object GetValue()
    {
        return Value;
    }
}
public class MessageTagString : MessageTag
{
    /// <summary>
    /// Строковое значение тега
    /// </summary>
    [MaxLength(100)]
    public string Value { get; set; } = string.Empty;

    public override object GetValue()
    {
        return Value;
    }
}
public class MessageTagDateTime : MessageTag
{
    /// <summary>
    /// Значение тега дата и время
    /// </summary>
    public DateTime Value { get; set; }

    public override object GetValue()
    {
        return Value;
    }
}
public class MessageTagBits : MessageTag
{
    /// <summary>
    /// Значение тега набор битов
    /// </summary>
    [MaxLength(32)]
    public BitArray Value { get; set; } = null!;

    public override object GetValue()
    {
        return Value;
    }
}
public class MessageTagByte : MessageTag
{
    /// <summary>
    /// Целочисленное значение тега длиной в 1 байт
    /// </summary>
    public byte Value { get; set; }

    public override object GetValue()
    {
        return Value;
    }
}
