namespace BioTonFMS.Expressions.Parsing;

public enum ErrorType
{
    EmptyExpression,
    UnexpectedSymbol
}

public class ParsingException : Exception
{
    public readonly Location Location;
    public readonly ErrorType ErrorType;

    public ParsingException(string message, Location location, ErrorType errorType) : base(message)
    {
        Location = location;
        ErrorType = errorType;
    }
}
