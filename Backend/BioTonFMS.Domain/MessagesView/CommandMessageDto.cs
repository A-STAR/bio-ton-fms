namespace BioTonFMS.Domain.MessagesView;

public class CommandMessageDto
{
    /// <summary>
    /// Идентификатор сообщения
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Порядковый номер сообщения в выборке
    /// </summary>
    public int Num { get; set; }

    /// <summary>
    /// Дата и время отправки сообщения с сервера (в UTC)
    /// </summary>
    public DateTime CommandDateTime { get; set; }

    /// <summary>
    /// Канал: отображает тип связи, который был использован для выполнения команды
    /// </summary>
    public string Channel { get; set; } = "TCP";

    /// <summary>
    /// Текст сообщения
    /// </summary>
    public string CommandText { get; set; }

    /// <summary>
    /// Дата и время выполнения команды (получения ответа)
    /// </summary>
    public DateTime? ExecutionTime { get; set; }

    /// <summary>
    /// Ответ, полученный от устройства на отправленную команду
    /// </summary>
    public string? CommandResponseText { get; set; }
}