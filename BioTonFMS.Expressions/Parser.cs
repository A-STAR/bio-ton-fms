using BioTonFMS.Expressions.AST;

namespace BioTonFMS.Expressions;

public static class Parser
{
    public static AstNode? ParseWithHandler(string expressionString, IExceptionHandler exceptionHandler)
    {
        try
        {
            return Parse(expressionString);
        }
        catch( Exception e )
        {
            if (!exceptionHandler.Handle(e, OperationTypeEnum.Parsing))
                throw;
        }
        return null;
    }

    public static AstNode Parse(string expressionString)
    {
        if (string.IsNullOrEmpty(expressionString))
            throw new ArgumentException("Expression shouldn't be empty!");
        var parser = new ExpressionParser();
        var result = parser.Parse(expressionString);
        return result;
    }
}
