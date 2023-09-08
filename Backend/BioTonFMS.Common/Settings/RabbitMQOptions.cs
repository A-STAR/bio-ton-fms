namespace BioTonFMS.Common.Settings;

/// <summary>
/// Настройки для интеграции с RabbitMQ
/// </summary>
public class RabbitMQOptions
{
    /// <summary>
    /// Адрес сервера RabbitMQ
    /// </summary>
    public string Host { get; set; } = "localhost";

    /// <summary>
    /// Порт сервера RabbitMQ
    /// </summary>
    public int Port { get; set; }

    /// <summary>
    /// Используемый VHost сервера RabbitMQ
    /// </summary>
    public string VHost { get; set; } = "/";

    /// <summary>
    /// логин RabbitMQ
    /// </summary>
    public string User { get; set; } = "guest";

    /// <summary>
    /// пароль RabbitMQ
    /// </summary>
    public string Password { get; set; } = "guest";

    /// <summary>
    /// количество попыток повторной обработки сообщения  
    /// </summary>
    public int DeliveryLimit { get; set; } = 0;

    /// <summary>
    /// Максимальное число необработанных сообщения от трекеров в очереди, после которого сообщения перестают приниматься
    /// </summary>
    public int? TrackerQueueMaxLength { get; set; } = null; 
}
