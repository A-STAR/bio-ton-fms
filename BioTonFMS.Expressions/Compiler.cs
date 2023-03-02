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

    /// <summary>
    /// Compiles the AST to expression tree using the passed parameter types
    /// </summary>
    /// <param name="node">The root of AST to compile</param>
    /// <param name="parameters">Names and types of available input parameters (variables).</param>
    /// <returns>Expression tree which is the result of compilation of AST</returns>
    public Expression Compile(AstNode node, IDictionary<string, Type> parameters)
    {
        var expressionParameters = GetParameters(node, parameters);
        var expression = CompileRec(node, expressionParameters);
        var lambda = Expression.Lambda(expression, expressionParameters.Values);
        return lambda;
    }

    /// <summary>
    /// Extracts parameter (variable) names from the expression's AST and converts them to a set
    /// of ParameterExpression objects taking in account the passed parameter types 
    /// </summary>
    /// <param name="node">Root of AST</param>
    /// <param name="parameters">Names and types of available parameters</param>
    /// <returns>Set of ParameterExpression objects</returns>
    /// <exception cref="ArgumentException">Is thrown when the passed set of parameter types
    /// contains unsupported types or when parameter referenced in the AST doesn't exist in the
    /// set of passed parameters</exception>
    private static IDictionary<string, ParameterExpression> GetParameters(AstNode node, IDictionary<string, Type> parameters)
    {
        var variables = node.GetVariables();
        var result = new Dictionary<string, ParameterExpression>(8);
        foreach (var name in variables)
        {
            if (parameters.TryGetValue(name, out var type))
            {
                if (type != typeof( TagData<double> ))
                    throw new ArgumentException($"Type {type} is not supported! Only TagData<double> is supported at the moment.", nameof(parameters));
                result.Add(name, Expression.Parameter(type, name));
            }
            else
            {
                throw new ArgumentException($"Variable with name {name} doesn't exist!", nameof(node));
            }
        }
        return result;
    }

    /// <summary>
    /// Recursively compiles the AST to expression tree 
    /// </summary>
    /// <param name="node">AST root</param>
    /// <param name="parameters">Dictionary which maps parameter names to their ParameterExpression objects</param>
    /// <returns>Resulting expression tree</returns>
    /// <exception cref="ArgumentException">Is thrown in cases when passed AST contains illegal enum values</exception>
    private Expression CompileRec(AstNode node, IDictionary<string, ParameterExpression> parameters)
    {
        return node switch
        {
            BinaryOperation v => v.Operation switch
            {
                BinaryOperationEnum.Addition =>
                    Expression.Add(CompileRec(v.LeftOperand, parameters), CompileRec(v.RightOperand, parameters))
                        .Adjust(),
                BinaryOperationEnum.Subtraction =>
                    Expression.Subtract(CompileRec(v.LeftOperand, parameters), CompileRec(v.RightOperand, parameters))
                        .Adjust(),
                BinaryOperationEnum.Multiplication =>
                    Expression.Multiply(CompileRec(v.LeftOperand, parameters), CompileRec(v.RightOperand, parameters))
                        .Adjust(),
                BinaryOperationEnum.Division =>
                    Expression.Divide(CompileRec(v.LeftOperand, parameters), CompileRec(v.RightOperand, parameters))
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
                LiteralEnum.Decimal => Expression.Constant(double.Parse(v.LiteralString, NumberStyles.Float, NumberFormatInfo.InvariantInfo),
                    typeof( double? )),
                _ => throw new ArgumentException("Invalid AST: Invalid type of literal!", nameof(v.Type))
            },
            Variable v => ExpressionExtensions.WrapParameter(parameters[v.Name], Options.UseFallbacks),
            _ => throw new ArgumentException("Invalid AST: Unknown type of node!")
        };
    }
}
