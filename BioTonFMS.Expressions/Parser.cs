using BioTonFMS.Expressions.AST;

namespace BioTonFMS.Expressions;

public static class Parser
{
    /// <summary>
    /// Executes parsing while using exception handler object to handle exceptions 
    /// </summary>
    /// <param name="expressionString">String which represents parsed expression</param>
    /// <param name="exceptionHandler">Object which handles exception thrown during parsing, compiling or execution.</param>
    /// <returns>AST representing parsed expression</returns>
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
    
    /// <summary>
    /// Parses expression string and translates it to abstract syntax tree 
    /// </summary>
    /// <param name="expressionString">String which represents parsed expression</param>
    /// <returns>AST representing parsed expression</returns>
    public static AstNode Parse(string expressionString)
    {
        if (string.IsNullOrEmpty(expressionString))
            throw new ArgumentException("Expression shouldn't be empty!");
        var parser = new ExpressionParser();
        var result = parser.Parse(expressionString);
        return result;
    }
}
