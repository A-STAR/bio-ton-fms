using BioTonFMS.Domain.TrackerMessages;

namespace BioTonFMS.TrackerMessageHandler.MessageParsing;

public interface IMessageParser
{
    IEnumerable<TrackerMessage> ParseMessage(byte[] binaryPackage, Guid packageUid);
}