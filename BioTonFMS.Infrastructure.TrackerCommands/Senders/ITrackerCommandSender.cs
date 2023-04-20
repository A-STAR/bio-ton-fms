using BioTonFMS.Domain;

namespace BioTonFMS.TrackerCommands.Senders;

public interface ITrackerCommandSender
{
    /// <summary>
    /// Отправляет команду трекеру
    /// </summary>
    /// <param name="tracker">Трекер, которому отправляется команда</param>
    /// <param name="commandText">Текст команды</param>
    /// <returns>
    /// Ответ от трекера, текст и (возможно) бинарные данные.
    /// Если в ответе не возвращаются бинарные данные, метод возвращает пустой массив.
    /// </returns>
    (string ResponseText, byte[] ResponseBinaryInfo) Send(Tracker tracker, string commandText);
}