using System.Globalization;
using System.Linq.Expressions;
using BioTonFMS.Expressions.AST;

namespace BioTonFMS.Expressions;

public class Compiler
{
    public readonly CompilerOptions Options;

    public Compiler(CompilerOptions options)
    {
        Options = options;
    }

    public Expression Compile(AstNode node, IDictionary<string, Type> parameters)
    {
        var expressionParameters = GetParameters(node, parameters);
        var expression = CompileRec(node, expressionParameters);
        var lambda = Expression.Lambda(expression, expressionParameters.Values);
        return lambda;
    }

    private static IDictionary<string, ParameterExpression> GetParameters(AstNode node, IDictionary<string, Type> parameters)
    {
        var variables = node.GetVariables();
        var result = new Dictionary<string, ParameterExpression>(8);
        foreach (var name in variables)
        {
            if (parameters.TryGetValue(name, out var type))
            {
                if (type != typeof( TagData<double> ))
                    throw new ArgumentException($"Type {type} is not supported! Only TagData<double> is supported at the moment.");
                result.Add(name, Expression.Parameter(type, name));
            }
            else
            {
                throw new FormatException($"Variable with name {name} doesn't exist!");
            }
        }
        return result;
    }

    private Expression CompileRec(AstNode node, IDictionary<string, ParameterExpression> parameters)
    {
        return node switch
        {
            BinaryOperation v => v.Operation switch
            {
                BinaryOperationEnum.Addition =>
                    Expression.Add(CompileRec(v.LeftNode, parameters), CompileRec(v.RightNode, parameters))
                        .Adjust(),
                BinaryOperationEnum.Subtraction =>
                    Expression.Subtract(CompileRec(v.LeftNode, parameters), CompileRec(v.RightNode, parameters))
                        .Adjust(),
                BinaryOperationEnum.Multiplication =>
                    Expression.Multiply(CompileRec(v.LeftNode, parameters), CompileRec(v.RightNode, parameters))
                        .Adjust(),
                BinaryOperationEnum.Division =>
                    Expression.Divide(CompileRec(v.LeftNode, parameters), CompileRec(v.RightNode, parameters))
                        .Adjust(),
                _ => throw new ArgumentException("Invalid AST: Invalid type of binary operation!", nameof(node))
            },
            UnaryOperation v => v.Operation switch
            {
                UnaryOperationEnum.Negation => Expression.Negate(CompileRec(v.Operand, parameters)).Adjust(),
                UnaryOperationEnum.Parentheses => CompileRec(v.Operand, parameters),
                _ => throw new ArgumentException("Invalid AST: Invalid type of unary operation!", nameof(v.Operation))
            },
            Literal v => v.Type switch
            {
                LiteralEnum.Decimal => Expression.Constant(double.Parse(v.Value, NumberStyles.Float, NumberFormatInfo.InvariantInfo),
                    typeof( double? )),
                _ => throw new ArgumentException("Invalid AST: Invalid type of literal!", nameof(v.Type))
            },
            Variable v => ExpressionExtensions.WrapParameter(parameters[v.Name], Options.UseFallbacks),
            _ => throw new ArgumentException("Invalid AST: Unknown type of node!")
        };
    }
}
