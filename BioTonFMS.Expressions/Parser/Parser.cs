using BioTonFMS.Expressions.AST;

namespace BioTonFMS.Expressions;

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
            throw new ArgumentException("Expression shouldn't be empty!");
        var parser = new ExpressionParser();
        var result = parser.Parse(expressionString);
        return result;
    }
}
