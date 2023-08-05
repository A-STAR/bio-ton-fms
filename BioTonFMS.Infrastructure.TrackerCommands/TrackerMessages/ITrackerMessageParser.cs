using BioTonFMS.Domain.TrackerMessages;

namespace BioTonFMS.TrackerProtocolSpecific.TrackerMessages;

public interface ITrackerMessageParser
{
    IEnumerable<TrackerMessage> ParseMessage(byte[] binaryPackage, Guid packageUid);
    bool IsCommandReply(byte[] binaryPackage);
}
