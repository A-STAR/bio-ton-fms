namespace BioTonFMS.TrackerMessageHandler.MessageParsing;

public interface IMessageParser
{
    void ParseMessage(byte[] binaryPackage);
}