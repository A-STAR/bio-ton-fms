using System.Linq.Expressions;
using BioTonFMS.Expressions.Ast;

namespace BioTonFMS.Expressions.Compilation;

public class Compiler
{
    private readonly IDictionary<string, Type> _parameterTypes;
    private readonly IExpressionBuilder _expressionBuilder;
    private IDictionary<string, ParameterExpression>? _parameterExpressions;

    public Compiler(IDictionary<string, Type> parameterTypes,
        CompilationOptions? options = null, IExpressionProperties? expressionProperties = null)
    {
        _parameterTypes = parameterTypes;
        _expressionBuilder = options?.ExpressionBuilderFactory == null
            ? new ExpressionBuilder(expressionProperties)
            : options.ExpressionBuilderFactory.Create(expressionProperties);
    }

    /// <summary>
    /// Compiles the AST to expression tree using the passed parameter types and compilation options
    /// </summary>
    /// <param name="node">The root of AST to compile</param>
    /// <param name="parameters">Names and types of available input parameters (variables).</param>
    /// <param name="options">Compilation options</param>
    /// <param name="expressionProperties"></param>
    public static LambdaExpression Compile(AstNode node, IDictionary<string, Type> parameters, CompilationOptions? options = null,
        IExpressionProperties? expressionProperties = null)
    {
        var compiler = new Compiler(parameters, options, expressionProperties);
        return compiler.Compile(node);
    }

    /// <summary>
    /// Compiles the AST to expression tree using the passed parameter types
    /// </summary>
    /// <param name="node">The root of AST to compile</param>
    /// <returns>Expression tree which is the result of compilation of AST</returns>
    public LambdaExpression Compile(AstNode node)
    {
        _parameterExpressions = BuildParameterExpressions(node);
        var expression = CompileRec(node);
        var wrappedExpression = _expressionBuilder.WrapExpression(expression);
        var lambda = _expressionBuilder.BuildLambda(wrappedExpression, _parameterExpressions.Values);
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
                if (!_expressionBuilder.IsParameterTypeSupported(type))
                    errors.Add(new CompilationError(ErrorType.UnsupportedTypeOfParameter, variable, type));
                
                result.Add(variable.Name, _expressionBuilder.BuildParameter(variable.Name, type));
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
            FunctionCall v => _expressionBuilder.BuildFunction(v.Name, v.Arguments.Select(CompileRec)),
            BinaryOperation v => _expressionBuilder.BuildBinary(v.Operation, CompileRec(v.LeftOperand), CompileRec(v.RightOperand)),
            UnaryOperation v => _expressionBuilder.BuildUnary(v.Operation, CompileRec(v.Operand)),
            Literal v => _expressionBuilder.BuildConstant(v.Type, v.LiteralString),
            Variable v => _expressionBuilder.WrapParameter(_parameterExpressions![v.Name]),
            _ => throw new Exception("Invalid AST: Unknown type of node!")
        };
    }
}
