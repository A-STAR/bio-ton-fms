using BioTonFMS.Domain;

namespace BioTonFMS.TrackerCommands.Codecs;

public interface ICommandCodec
{
    /// <summary>
    /// Возвращает пакет байт представляющих закодированную команду для заданного трекера.
    /// </summary>
    byte[] Encode(Tracker tracker, string commandText);

    /// <summary>
    /// Возвращает строку с ответом трекера, извлечённую из пакета данных ответа трекера,
    /// а так же массив бинарных данных, который может возвращаться с ответом на команду.
    /// Если в ответе не возвращаются бинарные данные, метод возвращает пустой массив.
    /// </summary>
    (string ResponseText, byte[] ResponseBynaryInfo) Decode(byte[] commandResponse);
}