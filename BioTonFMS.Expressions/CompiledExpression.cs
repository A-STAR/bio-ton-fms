using System.Linq.Expressions;

namespace BioTonFMS.Expressions;

public struct CompiledExpression<TExpressionProperties>
    where TExpressionProperties: Helpers.IExpressionProperties
{
    public readonly TExpressionProperties Properties;
    public readonly LambdaExpression? ExpressionTree;

    public CompiledExpression(TExpressionProperties properties, LambdaExpression? expressionTree)
    {
        Properties = properties;
        ExpressionTree = expressionTree;
    }
}
