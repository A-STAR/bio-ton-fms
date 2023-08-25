namespace BioTonFMS.TrackerProtocolSpecific.Exceptions;

public class ParseCommandResponseException : Exception
{
    public override string Message { get; }

    public ParseCommandResponseException(string reason)
    {
        Message = "Возникла ошибка в ходе разбора ответа трекера на команду: " + reason;
    }
}