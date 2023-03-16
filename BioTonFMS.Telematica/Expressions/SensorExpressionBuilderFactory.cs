using BioTonFMS.Expressions;
using BioTonFMS.Expressions.Compilation;

namespace BioTonFMS.Telematica.Expressions;

public class SensorExpressionBuilderFactory : IExpressionBuilderFactory
{
    public IExpressionBuilder Create(IExpressionProperties? expressionProperties) => new SensorExpressionBuilder(expressionProperties);
}
