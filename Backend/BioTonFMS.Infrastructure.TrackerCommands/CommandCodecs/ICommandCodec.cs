using BioTonFMS.Domain;

namespace BioTonFMS.TrackerProtocolSpecific.CommandCodecs;

public interface ICommandCodec
{
    /// <summary>
    /// Возвращает пакет байт представляющих закодированную команду для заданного трекера.
    /// </summary>
    byte[] EncodeCommand(Tracker tracker, int commandId, string commandText);

    /// <summary>
    /// Возвращает строку с ответом трекера, извлечённую из пакета данных ответа трекера,
    /// а так же массив бинарных данных, который может возвращаться с ответом на команду.
    /// Если в ответе не возвращаются бинарные данные, метод возвращает пустой массив.
    /// </summary>
    CommandResponseInfo DecodeCommand(byte[] commandResponse);
}