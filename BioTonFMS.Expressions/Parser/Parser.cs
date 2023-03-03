using BioTonFMS.Expressions.Ast;
using Pegasus.Common;

namespace BioTonFMS.Expressions.Parsing;

public static class Parser
{
    /// <summary>
    /// Parses expression string and translates it to abstract syntax tree 
    /// </summary>
    /// <param name="expressionString">String which represents parsed expression</param>
    /// <returns>AST representing parsed expression</returns>
    public static AstNode Parse(string expressionString)
    {
        if (string.IsNullOrEmpty(expressionString))
            throw new ParsingException("Expression shouldn't be empty!", new Location(0), ErrorType.EmptyExpression);
        var parser = new ExpressionParser();
        try
        {
            var result = parser.Parse(expressionString);
            return result;
        }
        catch( FormatException e )
        {
            var cursor = e.Data["cursor"] as Cursor;
            var location = cursor == null ? new Location(0) : new Location(cursor.Location);
            throw new ParsingException(e.Message, location, ErrorType.UnexpectedSymbol);
        }
    }
}
