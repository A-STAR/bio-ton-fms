using System.Globalization;
using BioTonFMS.Expressions;
using BioTonFMS.Expressions.AST;
using FluentAssertions;
using Xunit.Abstractions;

namespace BiotonFMS.Telematica.Tests.Expressions;

public class CompilerTests
{
    private ITestOutputHelper _outputHelper;
    
    public CompilerTests(ITestOutputHelper outputHelper)
    {
        _outputHelper = outputHelper;
        CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en");
    }

    private static IEnumerable<object[]> AstSamples => new[]
    {
        new object[]
        {
            new Literal("1", LiteralEnum.Decimal), "1", new object[] {1, 1, 1}
        },
        new object[]
        {
            new Literal("1.1", LiteralEnum.Decimal), "1.1", new object[] {1.1, 1.1, 1.1}
        },
        new object[]
        {
            new Literal("1e1", LiteralEnum.Decimal), "10", new object[] {10, 10, 10}
        },
        new object[]
        {
            new Literal("1e-1", LiteralEnum.Decimal), "0.1", new object[] {0.1, 0.1, 0.1}
        },
        new object[]
        {
            new Literal("-1", LiteralEnum.Decimal), "-1", new object[] {-1, -1, -1}
        },
        new object[]
        {
            new BinaryOperation(
                new Literal("1", LiteralEnum.Decimal),
                new Literal("1", LiteralEnum.Decimal),
                BinaryOperationEnum.Addition
                ),
            "IIF((1.HasValue And 1.HasValue), ConvertChecked((1 + 1), Nullable`1), null)",
            new object[] {2, 2, 2}
        },
        new object[]
        {
            new BinaryOperation(
                new Literal("1", LiteralEnum.Decimal),
                new Literal("1", LiteralEnum.Decimal),
                BinaryOperationEnum.Subtraction
                ),
            "IIF((1.HasValue And 1.HasValue), ConvertChecked((1 - 1), Nullable`1), null)",
            new object[] {0, 0, 0}
        },
        new object[]
        {
            new BinaryOperation(
                new Literal("2", LiteralEnum.Decimal),
                new Literal("3", LiteralEnum.Decimal),
                BinaryOperationEnum.Multiplication
                ),
            "IIF((2.HasValue And 3.HasValue), ConvertChecked((2 * 3), Nullable`1), null)",
            new object[] {6, 6, 6}
        },
        new object[]
        {
            new BinaryOperation(
                new Literal("1", LiteralEnum.Decimal),
                new Literal("100", LiteralEnum.Decimal),
                BinaryOperationEnum.Division
                ),
            "IIF((1.HasValue And 100.HasValue), ConvertChecked((1 / 100), Nullable`1), null)",
            new object[] {0.01, 0.01, 0.01}
        },
        new object[]
        {
            new UnaryOperation(
                new Literal("1", LiteralEnum.Decimal),
                UnaryOperationEnum.Negation
                ),
            "IIF(1.HasValue, ConvertChecked(-1, Nullable`1), null)",
            new object[] {-1, -1, -1}
        },
        new object[]
        {
            new UnaryOperation(
                new Literal("-1", LiteralEnum.Decimal),
                UnaryOperationEnum.Negation
                ),
            "IIF(-1.HasValue, ConvertChecked(--1, Nullable`1), null)",
            new object[] {1, 1, 1}
        },
        new object[]
        {
            new UnaryOperation(
                new Literal("1", LiteralEnum.Decimal),
                UnaryOperationEnum.Parentheses
                ),
            "1",
            new object[] {1, 1, 1}
        },
        new object[]
        {
            new Variable("a"), "IIF(a.IsFallback, null, a.Value)", new object?[] {1, null, 1}
        },
        new object[]
        {
            new Variable("b"), "IIF(b.IsFallback, null, b.Value)", new object?[] {2, null, null}
        },
        new object[]
        {
            new Variable("c"), "IIF(c.IsFallback, null, c.Value)", new object?[] {4, null, null}
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
            "IIF((IIF((IIF(a.IsFallback, null, a.Value).HasValue And IIF(b.IsFallback, null, b.Value).HasValue), ConvertChecked((IIF(a.IsFallback, null, a.Value) + IIF(b.IsFallback, null, b.Value)), Nullable`1), null).HasValue And IIF(c.IsFallback, null, c.Value).HasValue), ConvertChecked((IIF((IIF(a.IsFallback, null, a.Value).HasValue And IIF(b.IsFallback, null, b.Value).HasValue), ConvertChecked((IIF(a.IsFallback, null, a.Value) + IIF(b.IsFallback, null, b.Value)), Nullable`1), null) + IIF(c.IsFallback, null, c.Value)), Nullable`1), null)",
            new object?[] {7, null, null}
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
            "IIF((IIF(a.IsFallback, null, a.Value).HasValue And IIF((IIF(b.IsFallback, null, b.Value).HasValue And IIF(c.IsFallback, null, c.Value).HasValue), ConvertChecked((IIF(b.IsFallback, null, b.Value) + IIF(c.IsFallback, null, c.Value)), Nullable`1), null).HasValue), ConvertChecked((IIF(a.IsFallback, null, a.Value) + IIF((IIF(b.IsFallback, null, b.Value).HasValue And IIF(c.IsFallback, null, c.Value).HasValue), ConvertChecked((IIF(b.IsFallback, null, b.Value) + IIF(c.IsFallback, null, c.Value)), Nullable`1), null)), Nullable`1), null)",
            new object?[] {7, null, null}
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
            "IIF((IIF((IIF(a.IsFallback, null, a.Value).HasValue And IIF(b.IsFallback, null, b.Value).HasValue), ConvertChecked((IIF(a.IsFallback, null, a.Value) * IIF(b.IsFallback, null, b.Value)), Nullable`1), null).HasValue And IIF(c.IsFallback, null, c.Value).HasValue), ConvertChecked((IIF((IIF(a.IsFallback, null, a.Value).HasValue And IIF(b.IsFallback, null, b.Value).HasValue), ConvertChecked((IIF(a.IsFallback, null, a.Value) * IIF(b.IsFallback, null, b.Value)), Nullable`1), null) * IIF(c.IsFallback, null, c.Value)), Nullable`1), null)",
            new object?[] {8, null, null}
        },
        new object[]
        {
            new BinaryOperation(
                new Variable("a"),
                new Variable("b"),
                BinaryOperationEnum.Addition),
            "IIF((IIF(a.IsFallback, null, a.Value).HasValue And IIF(b.IsFallback, null, b.Value).HasValue), ConvertChecked((IIF(a.IsFallback, null, a.Value) + IIF(b.IsFallback, null, b.Value)), Nullable`1), null)",
            new object?[] {3, null, null}
        },
    };

    private static IEnumerable<object[]> ArgumentsSamples => new[]
    {
        new object[]
        {
            new Dictionary<string, object?>
            {
                {
                    "a", new TagData<double>(1)
                },
                {
                    "b", new TagData<double>(2)
                },
                {
                    "c", new TagData<double>(4)
                }
            },
            0
        },
        new object[]
        {
            new Dictionary<string, object?>
            {
                {
                    "a", new TagData<double>(1, true)
                },
                {
                    "b", new TagData<double>(2, true)
                },
                {
                    "c", new TagData<double>(4, true)
                }
            },
            1
        },
        new object[]
        {
            new Dictionary<string, object?>
            {
                {
                    "a", new TagData<double>(1)
                },
                {
                    "b", new TagData<double>(2, true)
                },
                {
                    "c", new TagData<double>(4, true)
                }
            },
            2
        }
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
    public void Compile_WellFormedAst_ReturnsCorrectExpressionTree(AstNode node, string expression, object[] calculationResults,
        Dictionary<string, object?> arguments, int calculationResultIndex)
    {
        _outputHelper.WriteLine($"Ast: {node}");
        _outputHelper.WriteLine($"Expr: {expression}");
        _outputHelper.WriteLine($"Args: {arguments}");
        _outputHelper.WriteLine($"Result: {calculationResults[calculationResultIndex]}");
        
        var parameters = arguments.ToDictionary(a => a.Key, a => a.Value!.GetType());
        
        _outputHelper.WriteLine($"Params:\n{parameters.Aggregate("", (a, p) => a + $"{p.Key}: {p.Value}\n")}");
        
        var compiler = new Compiler(new CompilerOptions()
        {
            UseFallbacks = false
        });
        var lambda = compiler.Compile(node, parameters);
        lambda.Should().NotBeNull();
        TestUtil.ExtractUnwrappedExpression(lambda)?.ToString().Should().Be(expression);
        var executionResult = Helpers.Execute(lambda, arguments);
        var value = ((object?)((TagData<double>?)executionResult)?.Value);
        value?.Should().Be(calculationResults[calculationResultIndex]);
    }
}
