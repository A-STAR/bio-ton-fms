using System.Globalization;
using System.Linq.Expressions;
using BioTonFMS.Expressions;
using BioTonFMS.Expressions.AST;
using FluentAssertions;

namespace BiotonFMS.Telematica.Tests.Expressions;

public class ExpressionCompilerTests
{
    public ExpressionCompilerTests()
    {
        CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en");
    }

    public static ICollection<object> AstSamples => new[]
    {
        new object[]
        {
            new Literal("1", LiteralEnum.Decimal), "1", 1
        },
        new object[]
        {
            new Literal("1.1", LiteralEnum.Decimal), "1.1", 1.1
        },
        new object[]
        {
            new Literal("1e1", LiteralEnum.Decimal), "10", 10
        },
        new object[]
        {
            new Literal("1e-1", LiteralEnum.Decimal), "0.1", 0.1
        },
        new object[]
        {
            new Literal("-1", LiteralEnum.Decimal), "-1", -1
        },
        new object[]
        {
            new BinaryOperation(
                new Literal("1", LiteralEnum.Decimal),
                new Literal("1", LiteralEnum.Decimal),
                BinaryOperationEnum.Addition
                ),
            "(1 + 1)",
            2
        },
        new object[]
        {
            new BinaryOperation(
                new Literal("1", LiteralEnum.Decimal),
                new Literal("1", LiteralEnum.Decimal),
                BinaryOperationEnum.Subtraction
                ),
            "(1 - 1)",
            0
        },
        new object[]
        {
            new BinaryOperation(
                new Literal("2", LiteralEnum.Decimal),
                new Literal("3", LiteralEnum.Decimal),
                BinaryOperationEnum.Multiplication
                ),
            "(2 * 3)",
            6
        },
        new object[]
        {
            new BinaryOperation(
                new Literal("1", LiteralEnum.Decimal),
                new Literal("100", LiteralEnum.Decimal),
                BinaryOperationEnum.Division
                ),
            "(1 / 100)",
            0.01
        },
        new object[]
        {
            new UnaryOperation(
                new Literal("1", LiteralEnum.Decimal),
                UnaryOperationEnum.Negation
                ),
            "-1",
            -1
        },
        new object[]
        {
            new UnaryOperation(
                new Literal("-1", LiteralEnum.Decimal),
                UnaryOperationEnum.Negation
                ),
            "--1",
            1
        },
        new object[]
        {
            new UnaryOperation(
                new Literal("1", LiteralEnum.Decimal),
                UnaryOperationEnum.Parentheses
                ),
            "1",
            1
        },
        new object[]
        {
            new Variable("a"), "a", 1
        },
        new object[]
        {
            new BinaryOperation(
                new BinaryOperation(
                    new Variable("a"),
                    new Variable("b"),
                    BinaryOperationEnum.Addition),
                new Variable("c"),
                BinaryOperationEnum.Addition),
            "((a + b) + c)",
            7
        },
        new object[]
        {
            new BinaryOperation(
                new Variable("a"),
                new UnaryOperation(
                    new BinaryOperation(
                        new Variable("b"),
                        new Variable("c"),
                        BinaryOperationEnum.Addition),
                    UnaryOperationEnum.Parentheses),
                BinaryOperationEnum.Addition),
            "(a + (b + c))",
            7
        },
        new object[]
        {
            new BinaryOperation(
                new BinaryOperation(
                    new Variable("a"),
                    new Variable("b"),
                    BinaryOperationEnum.Multiplication),
                new Variable("c"),
                BinaryOperationEnum.Multiplication),
            "((a * b) * c)",
            8
        },
    };

    [Theory, MemberData(nameof(AstSamples))]
    public void Compile_WellFormedAst_ReturnsCorrectExpressionTree(AstNode node, string expression, object calculationResult)
    {
        var parameters = new Dictionary<string, Type>
        {
            {
                "a", typeof( double )
            },
            {
                "b", typeof( double )
            },
            {
                "c", typeof( double )
            }
        };
        var arguments = new Dictionary<string, object>
        {
            {
                "a", 1
            },
            {
                "b", 2
            },
            {
                "c", 4
            }
        };

        var lambda = ExpressionCompiler.Compile(node, parameters) as LambdaExpression;
        lambda.Should().NotBeNull();
        lambda?.Body.ToString().Should().Be(expression);
        if (lambda is not null)
        {
            ExpressionCompiler.Execute(lambda, arguments).Should().Be(calculationResult);
        }
    }
}
