using BioTonFMS.Expressions;
using BioTonFMS.Expressions.Compilation;

namespace BiotonFMS.Telematica.Tests.Expressions;

public class ExpressionBuilderFactoryMock : IExpressionBuilderFactory
{
    public IExpressionProperties? ExpressionProperties;

    public IExpressionBuilder Create(IExpressionProperties? expressionProperties)
    {
        ExpressionProperties = expressionProperties;
        return new ExpressionBuilderMock();
    }
}
