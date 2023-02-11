using BioTonFMS.Expressions.AST;

namespace BioTonFMS.Expressions;

public static class Parser
{
    public static AstNode Parse(string expressionString)
    {
        if (string.IsNullOrEmpty(expressionString))
            throw new ArgumentException("Expression shouldn't be empty!");
        var parser = new ExpressionParser();
        var result = parser.Parse(expressionString);
        return result;
    }
}
