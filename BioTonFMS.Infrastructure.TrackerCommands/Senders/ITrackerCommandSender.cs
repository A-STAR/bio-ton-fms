using BioTonFMS.Domain;

namespace BioTonFMS.TrackerProtocolSpecific.Senders;

public interface ITrackerCommandSender
{
    /// <summary>
    /// Отправляет команду трекеру
    /// </summary>
    /// <returns>
    /// Ответ от трекера, текст и (возможно) бинарные данные.
    /// Если в ответе не возвращаются бинарные данные, метод возвращает пустой массив.
    /// </returns>
    void Send(TrackerCommand command);
}