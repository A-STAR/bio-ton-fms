using System.Linq.Expressions;
using BioTonFMS.Domain;
using BioTonFMS.Expressions.Compilation;
using BioTonFMS.MessageProcessing;
using FluentAssertions;

namespace BiotonFMS.Telematica.Tests.Expressions;

public class SensorExpressionBuilderTests
{
    [Fact]
    private void IsParameterTypeSupported_TagData_IsSupported()
    {
        var expressionBuilder = new SensorExpressionBuilder(new SensorExpressionProperties(new Sensor())) as IExpressionBuilder;
        var isSupported = expressionBuilder.IsParameterTypeSupported(typeof( TagData<double> ));
        isSupported.Should().BeTrue();
    }

    [Fact]
    private void IsParameterTypeSupported_Double_IsNotSupported()
    {
        var expressionBuilder = new SensorExpressionBuilder(new SensorExpressionProperties(new Sensor())) as IExpressionBuilder;
        var isSupported = expressionBuilder.IsParameterTypeSupported(typeof( double? ));
        isSupported.Should().BeFalse();
    }
    
    [Fact]
    private void WrapParameter_Fallback()
    {
        var expressionBuilder = new SensorExpressionBuilder(new SensorExpressionProperties(new Sensor()
        {
            UseLastReceived = true
        })) as IExpressionBuilder;
        var wrappedExpression = expressionBuilder.WrapParameter(Expression.Parameter(typeof( double? ), "a"));
        wrappedExpression.ToString().Should().Be("a.Value");
    }

    [Fact]
    private void WrapParameter_NoFallback()
    {
        var expressionBuilder = new SensorExpressionBuilder(new SensorExpressionProperties(new Sensor()
        {
            UseLastReceived = false
        })) as IExpressionBuilder;
        var wrappedExpression = expressionBuilder.WrapParameter(Expression.Parameter(typeof( TagData<double> ), "a"));
        wrappedExpression.ToString().Should().Be("IIF(a.IsFallback, null, a.Value)");
    }

    [Fact]
    private void WrapExpression()
    {
        var expressionBuilder = new SensorExpressionBuilder(new SensorExpressionProperties(new Sensor())) as IExpressionBuilder;
        var wrappedExpression = expressionBuilder.WrapExpression(Expression.Constant(1231d, typeof( double? )));
        wrappedExpression.ToString().Should().Be("new TagData`1(1231, False)");
    }

    [Fact]
    private void BuildLambda()
    {
        var expressionBuilder = new SensorExpressionBuilder(new SensorExpressionProperties(new Sensor())) as IExpressionBuilder;
        var wrappedExpression =
            expressionBuilder.BuildLambda(Expression.Constant(1231d, typeof( double? )), Array.Empty<ParameterExpression>());
        wrappedExpression.ToString().Should().Be("() => 1231");
    }
}
