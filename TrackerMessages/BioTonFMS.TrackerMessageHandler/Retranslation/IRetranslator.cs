namespace BioTonFMS.TrackerMessageHandler.Retranslation;

public interface IRetranslator
{
    Task Retranslate(byte[] rawMessage);
}