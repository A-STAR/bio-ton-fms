namespace BioTonFMS.Expressions.Compilation;

public interface IExpressionBuilderFactory
{
    IExpressionBuilder Create(IExpressionProperties? expressionProperties);
}
