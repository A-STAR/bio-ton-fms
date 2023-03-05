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

    private static readonly Expression<Func<double?, double?, double?>> GateExpression = (signal, gate) =>
        (gate ?? 0d) == 0d ? null : signal;

    Expression IExpressionBuilder.BuildFunction(string name, Expression[] arguments)
    {
        return name switch
        {
            "And" when arguments.Length == 2 => Expression.And(arguments[0], arguments[1]),
            "Or" when arguments.Length == 2 => Expression.Or(arguments[0], arguments[1]),
            "Gate" when arguments.Length == 2 => Expression.Invoke(GateExpression, arguments[0], arguments[1]),
            _ => base.BuildFunction(name, arguments)
        };
    }
}
