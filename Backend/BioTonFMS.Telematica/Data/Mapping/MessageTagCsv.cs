using CsvHelper.Configuration.Attributes;

namespace BioTonFMS.Domain.TrackerMessages;

public class MessageTagCsv
{
    /// <summary>
    /// Уникальный идентификатор записи
    /// </summary>
    [Name("id")]
    public new long Id { get; set; }

    /// <summary>
    /// Идентификатор сообщения
    /// </summary>
    [Name("tracker_message_id")]
    public long TrackerMessageId { get; set; }

    /// <summary>
    /// Идентификатор тега
    /// </summary>
    [Name("tracker_tag_id")]
    public int? TrackerTagId { get; set; }

    /// <summary>
    /// Дискриминатор
    /// </summary>
    [Name("tag_type")]
    public TagDataTypeEnum TagType { get; set; }

    /// <summary>
    /// Значение тега, представленное в виде строки
    /// </summary>
    [Name("value")] 
    public string? Value { get; set; }

    [Name("message_tag_boolean_value")]
    public bool? BooleanValue { get; set; }

    [Name("message_tag_byte_value")]
    public byte? ByteValue { get; set; }

    [Name("message_tag_date_time_value")]
    public DateTime? DateTimeValue { get; set; }

    [Name("message_tag_double_value")]
    public double? DoubleValue { get; set; }

    [Name("message_tag_integer_value")]
    public int? IntegerValue { get; set; }

    [Name("message_tag_string_value")]
    public string? StringValue { get; set; }

    /// <summary>
    /// Является ли значение значением взятым из прошлых сообщений
    /// </summary>
    [Name("is_fallback")]
    public bool IsFallback { get; set; }

    /// <summary>
    /// Идентификатор датчика
    /// </summary>
    [Name("sensor_id")]
    public int? SensorId { get; set; }
}
