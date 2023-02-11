using System.Collections;
using BioTonFMS.Expressions;
using BioTonFMS.Expressions.AST;
using FluentAssertions;

namespace BiotonFMS.Telematica.Tests.Expressions;

public class ParserTests
{
    [Fact]
    public void EmptyString_LeadsToArgumentException()
    {
        Action parse = () => Parser.Parse("");
        parse.Should().Throw<ArgumentException>();
    }

    public static ICollection<object> ParseSamples => new[]
    {
        new object[]
        {
            "const1", new Literal("1", LiteralEnum.Decimal)
        },
        new object[]
        {
            "const1.1", new Literal("1.1", LiteralEnum.Decimal)
        },
        new object[]
        {
            "const1e1", new Literal("1e1", LiteralEnum.Decimal)
        },
        new object[]
        {
            "const12345678790.1234567890e1234567890", new Literal("12345678790.1234567890e1234567890", LiteralEnum.Decimal)
        },
        new object[]
        {
            "const1e-1", new Literal("1e-1", LiteralEnum.Decimal)
        },
        new object[]
        {
            "const0e0", new Literal("0e0", LiteralEnum.Decimal)
        },
        new object[]
        {
            "const01e01", new Literal("01e01", LiteralEnum.Decimal)
        },
        new object[]
        {
            "const01e-01", new Literal("01e-01", LiteralEnum.Decimal)
        },
        new object[]
        {
            "const1 + const1",
            new BinaryOperation(
                new Literal("1", LiteralEnum.Decimal),
                new Literal("1", LiteralEnum.Decimal),
                BinaryOperationEnum.Addition
                )
        },
        new object[]
        {
            "const1+const1",
            new BinaryOperation(
                new Literal("1", LiteralEnum.Decimal),
                new Literal("1", LiteralEnum.Decimal),
                BinaryOperationEnum.Addition
                )
        },
        new object[]
        {
            "const1 - const1",
            new BinaryOperation(
                new Literal("1", LiteralEnum.Decimal),
                new Literal("1", LiteralEnum.Decimal),
                BinaryOperationEnum.Subtraction
                )
        },
        new object[]
        {
            "const1-const1",
            new BinaryOperation(
                new Literal("1", LiteralEnum.Decimal),
                new Literal("1", LiteralEnum.Decimal),
                BinaryOperationEnum.Subtraction
                )
        },
        new object[]
        {
            "const1 * const1",
            new BinaryOperation(
                new Literal("1", LiteralEnum.Decimal),
                new Literal("1", LiteralEnum.Decimal),
                BinaryOperationEnum.Multiplication
                )
        },
        new object[]
        {
            "const1*const1",
            new BinaryOperation(
                new Literal("1", LiteralEnum.Decimal),
                new Literal("1", LiteralEnum.Decimal),
                BinaryOperationEnum.Multiplication
                )
        },
        new object[]
        {
            "const1 / const1",
            new BinaryOperation(
                new Literal("1", LiteralEnum.Decimal),
                new Literal("1", LiteralEnum.Decimal),
                BinaryOperationEnum.Division
                )
        },
        new object[]
        {
            "const1/const1",
            new BinaryOperation(
                new Literal("1", LiteralEnum.Decimal),
                new Literal("1", LiteralEnum.Decimal),
                BinaryOperationEnum.Division
                )
        },
        new object[]
        {
            "- const1",
            new UnaryOperation(
                new Literal("1", LiteralEnum.Decimal),
                UnaryOperationEnum.Negation
                )
        },
        new object[]
        {
            "-const1",
            new UnaryOperation(
                new Literal("1", LiteralEnum.Decimal),
                UnaryOperationEnum.Negation
                )
        },
        new object[]
        {
            "-const-1",
            new UnaryOperation(
                new Literal("-1", LiteralEnum.Decimal),
                UnaryOperationEnum.Negation
                )
        },
        new object[]
        {
            "( const1 )",
            new UnaryOperation(
                new Literal("1", LiteralEnum.Decimal),
                UnaryOperationEnum.Parentheses
                )
        },
        new object[]
        {
            "(const1)",
            new UnaryOperation(
                new Literal("1", LiteralEnum.Decimal),
                UnaryOperationEnum.Parentheses
                )
        },
        new object[]
        {
            "abcdefgjijklmnopqrstuvwxyz_ABCDEFGJIJKLMNOPQRSTUVWXYZ_1234567890",
            new Variable("abcdefgjijklmnopqrstuvwxyz_ABCDEFGJIJKLMNOPQRSTUVWXYZ_1234567890")
        },
        new object[]
        {
            "a + b + c",
            new BinaryOperation(
                new BinaryOperation(
                    new Variable("a"),
                    new Variable("b"),
                    BinaryOperationEnum.Addition),
                new Variable("c"),
                BinaryOperationEnum.Addition)
        },
        new object[]
        {
            "a + (b + c)",
            new BinaryOperation(
                new Variable("a"),
                new UnaryOperation(
                    new BinaryOperation(
                        new Variable("b"),
                        new Variable("c"),
                        BinaryOperationEnum.Addition),
                    UnaryOperationEnum.Parentheses),
                BinaryOperationEnum.Addition)
        },
        new object[]
        {
            "a * (b * c)",
            new BinaryOperation(
                new Variable("a"),
                new UnaryOperation(
                    new BinaryOperation(
                        new Variable("b"),
                        new Variable("c"),
                        BinaryOperationEnum.Multiplication),
                    UnaryOperationEnum.Parentheses),
                BinaryOperationEnum.Multiplication)
        },
        new object[]
        {
            "a * b * c",
            new BinaryOperation(
                new BinaryOperation(
                    new Variable("a"),
                    new Variable("b"),
                    BinaryOperationEnum.Multiplication),
                new Variable("c"),
                BinaryOperationEnum.Multiplication)
        },
        new object[]
        {
            "a + b * c",
            new BinaryOperation(
                new Variable("a"),
                new BinaryOperation(
                    new Variable("b"),
                    new Variable("c"),
                    BinaryOperationEnum.Multiplication),
                BinaryOperationEnum.Addition)
        },
        new object[]
        {
            "a + b * c",
            new BinaryOperation(
                new Variable("a"),
                new BinaryOperation(
                    new Variable("b"),
                    new Variable("c"),
                    BinaryOperationEnum.Multiplication),
                BinaryOperationEnum.Addition)
        },
        new object[]
        {
            "a * b + c",
            new BinaryOperation(
                new BinaryOperation(
                    new Variable("a"),
                    new Variable("b"),
                    BinaryOperationEnum.Multiplication),
                new Variable("c"),
                BinaryOperationEnum.Addition)
        },
    };

    [Theory, MemberData(nameof(ParseSamples))]
    public void Parse_SyntacticallyCorrectExpression_GetValidAst(string expressionString, AstNode expectedAst)
    {
        var ast = Parser.Parse(expressionString);
        ast.Should().NotBeNull();
        ast.Should().BeAssignableTo<AstNode>();
        ast.Should().Be(expectedAst);
    }

    public static ICollection<object> SyntacticallyIncorrectSamples => new[]
    {
        new object[]
        {
            "ё"
        },
        new object[]
        {
            "a+"
        },
    };

    [Theory, MemberData(nameof(SyntacticallyIncorrectSamples))]
    public void Parse_SyntacticallyIncorrectExpression_Throws(string expressionString)
    {
        Action parse = () => Parser.Parse(expressionString);
        parse.Should().Throw<FormatException>();
    }

}
