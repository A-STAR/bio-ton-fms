using System.Linq.Expressions;
using BioTonFMS.Expressions;
using BioTonFMS.Expressions.Compilation;

namespace BioTonFMS.Telematica.Expressions;

public class SensorExpressionBuilder : ExpressionBuilder, IExpressionBuilder
{
    private readonly IExpressionProperties? _expressionProperties;

    public SensorExpressionBuilder(IExpressionProperties? expressionProperties)
    {
        _expressionProperties = expressionProperties;
    }
    
    bool IExpressionBuilder.IsParameterTypeSupported(Type type) => type == typeof( TagData<double> );
   

    Expression IExpressionBuilder.WrapParameter(ParameterExpression parameterExpression)
    {
        var props = _expressionProperties as ISensorExpressionProperties;
        return props is not { UseFallbacks: true } ? parameterExpression.Current() : parameterExpression.Latest();
    }
    
    Expression IExpressionBuilder.WrapExpression(Expression expression)
    {
        return Expression.New(typeof( TagData<double> ).GetConstructors()[0], expression, Expression.Constant(false));
    }
    
    Expression IExpressionBuilder.BuildFunction(string name, IEnumerable<Expression> arguments)
    {
        throw new Exception($"There is no function with name {name}!");
    }
}
