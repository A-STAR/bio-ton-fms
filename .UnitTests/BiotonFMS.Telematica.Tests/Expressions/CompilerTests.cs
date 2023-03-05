using System.Globalization;
using BioTonFMS.Expressions;
using BioTonFMS.Expressions.Ast;
using BioTonFMS.Expressions.Compilation;
using FluentAssertions;
using Xunit.Abstractions;

namespace BiotonFMS.Telematica.Tests.Expressions;

public class CompilerTests
{
    private readonly ITestOutputHelper _outputHelper;

    public CompilerTests(ITestOutputHelper outputHelper)
    {
        _outputHelper = outputHelper;
        CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en");
    }

    private static IEnumerable<object[]> AstSamples => new[]
    {
        new object[]
        {
            new Literal("1", LiteralEnum.Decimal),
            "1",
            new object[]
            {
                1, 1, 1
            }
        },
        new object[]
        {
            new Literal("1.1", LiteralEnum.Decimal),
            "1.1",
            new object[]
            {
                1.1, 1.1, 1.1
            }
        },
        new object[]
        {
            new Literal("1e1", LiteralEnum.Decimal),
            "10",
            new object[]
            {
                10, 10, 10
            }
        },
        new object[]
        {
            new Literal("1e-1", LiteralEnum.Decimal),
            "0.1",
            new object[]
            {
                0.1, 0.1, 0.1
            }
        },
        new object[]
        {
            new Literal("-1", LiteralEnum.Decimal),
            "-1",
            new object[]
            {
                -1, -1, -1
            }
        },
        new object[]
        {
            new BinaryOperation(
                new Literal("1", LiteralEnum.Decimal),
                new Literal("1", LiteralEnum.Decimal),
                BinaryOperationEnum.Addition
                ),
            "(1 + 1)",
            new object[]
            {
                2, 2, 2
            }
        },
        new object[]
        {
            new BinaryOperation(
                new Literal("1", LiteralEnum.Decimal),
                new Literal("1", LiteralEnum.Decimal),
                BinaryOperationEnum.Subtraction
                ),
            "(1 - 1)",
            new object[]
            {
                0, 0, 0
            }
        },
        new object[]
        {
            new BinaryOperation(
                new Literal("2", LiteralEnum.Decimal),
                new Literal("3", LiteralEnum.Decimal),
                BinaryOperationEnum.Multiplication
                ),
            "(2 * 3)",
            new object[]
            {
                6, 6, 6
            }
        },
        new object[]
        {
            new BinaryOperation(
                new Literal("1", LiteralEnum.Decimal),
                new Literal("100", LiteralEnum.Decimal),
                BinaryOperationEnum.Division
                ),
            "(1 / 100)",
            new object[]
            {
                0.01, 0.01, 0.01
            }
        },
        new object[]
        {
            new UnaryOperation(
                new Literal("1", LiteralEnum.Decimal),
                UnaryOperationEnum.Negation
                ),
            "-1",
            new object[]
            {
                -1, -1, -1
            }
        },
        new object[]
        {
            new UnaryOperation(
                new Literal("-1", LiteralEnum.Decimal),
                UnaryOperationEnum.Negation
                ),
            "--1",
            new object[]
            {
                1, 1, 1
            }
        },
        new object[]
        {
            new UnaryOperation(
                new Literal("1", LiteralEnum.Decimal),
                UnaryOperationEnum.Parentheses
                ),
            "1",
            new object[]
            {
                1, 1, 1
            }
        },
        new object[]
        {
            new Variable("a"),
            "a",
            new object?[]
            {
                1, null, 1
            }
        },
        new object[]
        {
            new Variable("b"),
            "b",
            new object?[]
            {
                2, 2, null
            }
        },
        new object[]
        {
            new Variable("c"),
            "c",
            new object?[]
            {
                4, 4, 4
            }
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
            new object?[]
            {
                7, null, null
            }
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
            new object?[]
            {
                7, null, null
            }
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
            new object?[]
            {
                8, null, null
            }
        },
        new object[]
        {
            new BinaryOperation(
                new Variable("a"),
                new Variable("b"),
                BinaryOperationEnum.Addition),
            "(a + b)",
            new object?[]
            {
                3, null, null
            }
        },
    };

    private static IEnumerable<object[]> ArgumentsSamples => new[]
    {
        new object[]
        {
            new Dictionary<string, object?>
            {
                {
                    "a", 1d
                },
                {
                    "b", 2d
                },
                {
                    "c", 4d
                }
            },
            0
        },
        new object[]
        {
            new Dictionary<string, object?>
            {
                {
                    "a", null
                },
                {
                    "b", 2d
                },
                {
                    "c", 4d
                }
            },
            1
        },
        new object[]
        {
            new Dictionary<string, object?>
            {
                {
                    "a", 1d
                },
                {
                    "b", null
                },
                {
                    "c", 4d
                }
            },
            2
        },
    };

    public static IEnumerable<object[]> AstArgumentsSamples
    {
        get
        {
            foreach (var ast in AstSamples)
            {
                foreach (var arguments in ArgumentsSamples)
                {
                    var concat = new object[5];
                    ast.CopyTo(concat, 0);
                    arguments.CopyTo(concat, 3);
                    yield return concat;
                }
            }
        }
    }

    [Theory, MemberData(nameof(AstArgumentsSamples))]
    public void Compile_WellFormedAstAndCorrectParameters_ReturnsCorrectExpressionTree(AstNode node, string expression,
        object?[] calculationResults,
        Dictionary<string, object?> arguments, int calculationResultIndex)
    {
        _outputHelper.WriteLine($"Ast: {node}");
        _outputHelper.WriteLine($"Expr: {expression}");
        _outputHelper.WriteLine($"Args: {string.Join(", ", arguments)}");
        _outputHelper.WriteLine($"Result: {calculationResults[calculationResultIndex] ?? "null"}, index: {calculationResultIndex}");

        var parameters = arguments.ToDictionary(a => a.Key, _ => typeof(double?));

        _outputHelper.WriteLine($"Params:\n{parameters.Aggregate("", (a, p) => a + $"{p.Key}: {p.Value}\n")}");

        var lambda = Compiler.Compile(node, parameters, new CompilationOptions() { ExpressionBuilderFactory = new ExpressionBuilderFactoryMock() });
        lambda.Should().NotBeNull();
        TestUtil.ExtractUnwrappedExpression(lambda)?.ToString().Should().Be(expression);

        var compiledExpression = new CompiledExpression<ExpressionPropertiesMock>(new ExpressionPropertiesMock(), lambda);
        var executionResult = Helpers.Execute(compiledExpression, arguments);
        executionResult.Should().Be(calculationResults[calculationResultIndex]);
    }

    [Fact]
    public void Compile_BadParameters_ThrowsCompilationException()
    {
        var unsupportedType = typeof( List<int> );
        var parameters = new Dictionary<string, Type>()
        {
            {
                "a", unsupportedType
            }
        };
        var leftOperand = new Variable("a");
        var rightOperand = new Variable("b");
        var ast = new BinaryOperation(leftOperand, rightOperand, BinaryOperationEnum.Addition);

        Action action = () => Compiler.Compile(ast, parameters);

        action.Should()
            .Throw<CompilationException>()
            .And.CompilationErrors.Should().Contain(new CompilationError[]
            {
                new(ErrorType.ParameterDoesNotExist, rightOperand, null),
                new(ErrorType.UnsupportedTypeOfParameter, leftOperand,
                    unsupportedType),
            });
    }

    [Fact]
    public void Compile_PassesExpressionPropertiesToExpressionBuilderFactory()
    {
        var parameters = new Dictionary<string, Type>();
        var expressionBuilderFactoryMock = new ExpressionBuilderFactoryMock();

        var _ = new Compiler(parameters,
            new CompilationOptions()
            {
                ExpressionBuilderFactory = expressionBuilderFactoryMock
            },
            new ExpressionPropertiesMock("mock1", string.Empty, true));

        expressionBuilderFactoryMock.ExpressionProperties?.Name.Should().Be("mock1");
        expressionBuilderFactoryMock.ExpressionProperties?.UseFallbacks.Should().BeTrue();
    }
}

