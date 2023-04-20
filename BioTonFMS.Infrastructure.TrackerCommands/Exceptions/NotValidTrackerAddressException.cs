namespace BioTonFMS.TrackerCommands.Exceptions;

public class NotValidTrackerAddressException : Exception
{
    public override string Message { get; }

    public NotValidTrackerAddressException()
    {
        Message = "IP адрес трекера устарел или не был установлен";
    }
}