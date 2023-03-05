using BioTonFMS.Expressions;
using BioTonFMS.Expressions.Compilation;
using BioTonFMS.Telematica.Expressions;

namespace BiotonFMS.Telematica.Tests.Expressions;

public class ExpressionBuilderFactoryMock : IExpressionBuilderFactory
{
    public ISensorExpressionProperties? ExpressionProperties;

    public IExpressionBuilder Create(IExpressionProperties? expressionProperties)
    {
        ExpressionProperties = expressionProperties as ISensorExpressionProperties;
        return new ExpressionBuilderMock();
    }
}
