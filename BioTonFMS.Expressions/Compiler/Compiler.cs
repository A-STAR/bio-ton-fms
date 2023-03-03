using System.Globalization;
using System.Linq.Expressions;
using BioTonFMS.Expressions.Ast;

namespace BioTonFMS.Expressions.Compilation;

public class Compiler
{
    private readonly IDictionary<string, Type> _parameterTypes;
    private readonly CompilationOptions _options;
    private IDictionary<string, ParameterExpression>? _parameterExpressions;

    private Compiler(CompilationOptions options, IDictionary<string, Type> parameterTypes)
    {
        _options = options;
        _parameterTypes = parameterTypes;
    }

    /// <summary>
    /// Compiles the AST to expression tree using the passed parameter types and compilation options
    /// </summary>
    /// <param name="node">The root of AST to compile</param>
    /// <param name="parameters">Names and types of available input parameters (variables).</param>
    /// <param name="options">Compilation options</param>
    public static LambdaExpression Compile(AstNode node, IDictionary<string, Type> parameters, CompilationOptions options)
    {
        var compiler = new Compiler(options, parameters);
        return compiler.Compile(node);
    }

    /// <summary>
    /// Compiles the AST to expression tree using the passed parameter types
    /// </summary>
    /// <param name="node">The root of AST to compile</param>
    /// <returns>Expression tree which is the result of compilation of AST</returns>
    private LambdaExpression Compile(AstNode node)
    {
        _parameterExpressions = BuildParameterExpressions(node);
        var expression = CompileRec(node);
        var wrappedExpression = Expression.New(typeof( TagData<double> ).GetConstructors()[0], expression, Expression.Constant(false));
        var lambda = Expression.Lambda(wrappedExpression, _parameterExpressions.Values);
        return lambda;
    }

    /// <summary>
    /// Extracts parameter (variable) names from the expression's AST and converts them to a set
    /// of ParameterExpression objects taking in account the passed parameter types 
    /// </summary>
    /// <param name="node">Root of AST</param>
    /// <returns>Set of ParameterExpression objects</returns>
    /// <exception cref="ArgumentException">Is thrown when the passed set of parameter types
    /// contains unsupported types or when parameter referenced in the AST doesn't exist in the
    /// set of passed parameters</exception>
    private IDictionary<string, ParameterExpression> BuildParameterExpressions(AstNode node)
    {
        var variables = node.GetVariables();
        var result = new Dictionary<string, ParameterExpression>(8);
        var errors = new List<CompilationError>();
        foreach (var variable in variables)
        {
            if (_parameterTypes.TryGetValue(variable.Name, out var type))
            {
                if (type != typeof( TagData<double> ))
                    errors.Add(new CompilationError(ErrorType.UnsupportedTypeOfParameter, variable, type));
                result.Add(variable.Name, Expression.Parameter(type, variable.Name));
            }
            else
                errors.Add(new CompilationError(ErrorType.ParameterDoesNotExist, variable, null));
        }
        
        if (errors.Count > 0)
            throw new CompilationException($"Compilation errors!", errors);

        return result;
    }

    /// <summary>
    /// Recursively compiles the AST to expression tree 
    /// </summary>
    /// <param name="node">AST root</param>
    /// <returns>Resulting expression tree</returns>
    /// <exception cref="ArgumentException">Is thrown in cases when passed AST contains illegal enum values</exception>
    private Expression CompileRec(AstNode node)
    {
        return node switch
        {
            BinaryOperation v => v.Operation switch
            {
                BinaryOperationEnum.Addition =>
                    Expression.Add(CompileRec(v.LeftOperand), CompileRec(v.RightOperand))
                        .Adjust(),
                BinaryOperationEnum.Subtraction =>
                    Expression.Subtract(CompileRec(v.LeftOperand), CompileRec(v.RightOperand))
                        .Adjust(),
                BinaryOperationEnum.Multiplication =>
                    Expression.Multiply(CompileRec(v.LeftOperand), CompileRec(v.RightOperand))
                        .Adjust(),
                BinaryOperationEnum.Division =>
                    Expression.Divide(CompileRec(v.LeftOperand), CompileRec(v.RightOperand))
                        .Adjust(),
                _ => throw new Exception("Invalid AST: Invalid type of binary operation!")
            },
            UnaryOperation v => v.Operation switch
            {
                UnaryOperationEnum.Negation => Expression.Negate(CompileRec(v.Operand)).Adjust(),
                UnaryOperationEnum.Parentheses => CompileRec(v.Operand),
                _ => throw new Exception("Invalid AST: Invalid type of unary operation!")
            },
            Literal v => v.Type switch
            {
                LiteralEnum.Decimal => Expression.Constant(
                    double.Parse(v.LiteralString, NumberStyles.Float, NumberFormatInfo.InvariantInfo),
                    typeof( double? )),
                _ => throw new Exception("Invalid AST: Invalid type of literal!")
            },
            Variable v => ExpressionBuilder.WrapParameter(_parameterExpressions![v.Name], _options.UseFallbacks),
            _ => throw new Exception("Invalid AST: Unknown type of node!")
        };
    }
}
