using System.Linq.Expressions;
using BioTonFMS.Expressions.Ast;
using BioTonFMS.Expressions.Compilation;
using FluentAssertions;

namespace BiotonFMS.Telematica.Tests.Expressions;

public class ExpressionBuilderTests
{
    [Fact]
    private void IsParameterTypeSupported_TagData_IsNotSupported()
    {
        var expressionBuilder = new ExpressionBuilder() as IExpressionBuilder;
        var isSupported = expressionBuilder.IsParameterTypeSupported(typeof( TagData<double> ));
        isSupported.Should().BeFalse();
    }

    [Fact]
    private void IsParameterTypeSupported_Double_IsSupported()
    {
        var expressionBuilder = new ExpressionBuilder() as IExpressionBuilder;
        var isSupported = expressionBuilder.IsParameterTypeSupported(typeof( double? ));
        isSupported.Should().BeTrue();
    }

    [Fact]
    private void WrapExpression()
    {
        var expressionBuilder = new ExpressionBuilder() as IExpressionBuilder;
        var wrappedExpression = expressionBuilder.WrapExpression(Expression.Constant(1231d, typeof( double? )));
        wrappedExpression.ToString().Should().Be("1231");
    }

    [Fact]
    private void BuildLambda()
    {
        var expressionBuilder = new ExpressionBuilder() as IExpressionBuilder;
        var wrappedExpression =
            expressionBuilder.BuildLambda(Expression.Constant(1231d, typeof( double? )), Array.Empty<ParameterExpression>());
        wrappedExpression.ToString().Should().Be("() => 1231");
    }

    [Fact]
    public void BuildBinary()
    {
        var expressionBuilder = new ExpressionBuilder() as IExpressionBuilder;
        var wrappedExpression = expressionBuilder.BuildBinary(BinaryOperationEnum.Addition, Expression.Constant(1d, typeof( double? )),
            Expression.Constant(2d, typeof( double? )));
        wrappedExpression.ToString().Should().Be("(1 + 2)");
    }

    [Fact]
    public void BuildUnary()
    {
        var expressionBuilder = new ExpressionBuilder() as IExpressionBuilder;
        var wrappedExpression = expressionBuilder.BuildUnary(UnaryOperationEnum.Negation, Expression.Constant(1d, typeof(double?)));
        wrappedExpression.ToString().Should().Be("-1");
    }

    [Fact]
    public void BuildFunction()
    {
        var expressionBuilder = new ExpressionBuilder() as IExpressionBuilder;
        expressionBuilder
            .Invoking(b => b.BuildFunction("SomeFunction", Array.Empty<Expression>()))
            .Should().Throw<Exception>().WithMessage("*no function*");
    }

    [Fact]
    public void BuildConstant()
    {
        var expressionBuilder = new ExpressionBuilder() as IExpressionBuilder;
        var wrappedExpression = expressionBuilder.BuildConstant(LiteralEnum.Decimal, "1");
        wrappedExpression.ToString().Should().Be("1");
    }
    [Fact]
    public void WrapParameter()
    {
        var expressionBuilder = new ExpressionBuilder() as IExpressionBuilder;
        var wrappedExpression = expressionBuilder.WrapParameter(Expression.Parameter(typeof(double?), "a"));
        wrappedExpression.ToString().Should().Be("a");
    }
    [Fact]
    public void BuildParameter()
    {
        var expressionBuilder = new ExpressionBuilder() as IExpressionBuilder;
        var wrappedExpression = expressionBuilder.BuildParameter("a", typeof(double?));
        wrappedExpression.ToString().Should().Be("a");
    }
}
