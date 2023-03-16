using System.Linq.Expressions;

namespace BioTonFMS.Expressions;

public struct CompiledExpression<TExpressionProperties>
    where TExpressionProperties : IExpressionProperties
{
    public readonly TExpressionProperties Properties;
    public readonly LambdaExpression? ExpressionTree;
    public Delegate? CompiledDelegate = null;

    public CompiledExpression(TExpressionProperties properties, LambdaExpression? expressionTree)
    {
        Properties = properties;
        ExpressionTree = expressionTree;
    }
    public Delegate? Compile() => CompiledDelegate ??= ExpressionTree?.Compile();
}
