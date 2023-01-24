namespace BioTonFMS.Telematica.MessageParsing;

public interface IMessageParser
{
    void ParseMessage(byte[] binaryPackage);
}