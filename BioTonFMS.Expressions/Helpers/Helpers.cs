using System.Linq.Expressions;
using BioTonFMS.Expressions.Ast;
using BioTonFMS.Expressions.Parsing;

namespace BioTonFMS.Expressions;

public static class Helpers
{
    /// <summary>
    /// Executes parsing while using exception handler object to handle exceptions 
    /// </summary>
    /// <param name="expressionString">String which represents parsed expression</param>
    /// <param name="exceptionHandler">Object which handles exception thrown during parsing, compiling or execution.</param>
    /// <returns>AST representing parsed expression</returns>
    private static AstNode? ParseWithHandler(string expressionString, IExceptionHandler exceptionHandler)
    {
        try
        {
            return Parser.Parse(expressionString);
        }
        catch( Exception e )
        {
            if (!exceptionHandler.Handle(e, OperationTypeEnum.Parsing))
                throw;
        }
        return null;
    }

    /// <summary>
    /// Compiles expression while using passed exception handler to handle exceptions
    /// </summary>
    /// <param name="node">Root of AST to compile</param>
    /// <param name="parameters">Names and types of available input parameters.</param>
    /// <param name="exceptionHandler">Object which handles exception thrown during parsing, compiling or execution.</param>
    /// <param name="options">CompilationOptions</param>
    /// <returns>Expression tree which is the result of compilation of AST</returns>
    private static LambdaExpression? CompileWithHandler(this AstNode node, IDictionary<string, Type> parameters,
        IExceptionHandler exceptionHandler, CompilationOptions options)
    {
        try
        {
            return Compiler.Compile(node, parameters, options);
        }
        catch( Exception e )
        {
            if (!exceptionHandler.Handle(e, OperationTypeEnum.Parsing))
                throw;
        }
        return null;
    }

    /// <summary>
    /// Builds set of mutually dependent expressions
    /// </summary>
    /// <param name="expressions">Sequence of pairs of expressions and their names: (name1, expression1), (name2, expression2),...
    /// Expressions may reference other expressions by their names</param>
    /// <param name="parameters">Types of parameters of the expressions by name</param>
    /// <param name="exceptionHandler">Object which handles exception thrown during parsing, compiling or execution.</param>
    /// <returns>Set of compiled expressions with their names: (name1, expression1), (name2, expression2),...</returns>
    public static IEnumerable<CompiledExpression<TExpressionProperties>> SortAndBuild<TExpressionProperties>(
        this IEnumerable<TExpressionProperties> expressions, IDictionary<string, Type> parameters,
        IExceptionHandler? exceptionHandler = null)
        where TExpressionProperties : IExpressionProperties
    {
        exceptionHandler ??= new DefaultExceptionHandler();

        var graph = expressions
            .ToDictionary(
                keySelector: e => e.Name,
                elementSelector: e =>
                {
                    var ast = ParseWithHandler(e.Formula, exceptionHandler);
                    return (
                        Edges: ast is null
                            ? Array.Empty<string>()
                            : (ICollection<string>)ast.GetVariables().Select(v => v.Name).ToArray(),
                        Data: (Ast: ast, Props: e));
                });

        var allParameters = new Dictionary<string, Type>(parameters);

        var sortedExpressionNames = graph.TopologicalSort();
        sortedExpressionNames.Reverse();
        return sortedExpressionNames
            .Select(name =>
            {
                var node = graph[name];
                var options = new CompilationOptions
                {
                    UseFallbacks = node.Data.Props.UseFallbacks
                };
                var compiledExpression = node.Data.Ast?.CompileWithHandler(allParameters, exceptionHandler, options);
                if (compiledExpression is not null) allParameters.Add(name, compiledExpression.ReturnType);
                return new CompiledExpression<TExpressionProperties>(node.Data.Props, compiledExpression);
            });
    }

    /// <summary>
    /// Executes lambda expression with specified arguments
    /// </summary>
    /// <param name="compiledExpression">Compiled lambda expression</param>
    /// <param name="arguments">Dictionary of named arguments for lambda. It may contain more arguments than required by the lambda</param>
    /// <returns>Result of execution of the lambda expression</returns>
    public static object? Execute<TExpressionProperties>(CompiledExpression<TExpressionProperties> compiledExpression,
        IDictionary<string, object?> arguments)
        where TExpressionProperties : IExpressionProperties
    {
        if (compiledExpression.ExpressionTree == null)
            return null;
        var args = compiledExpression.ExpressionTree.Parameters.Select(p => arguments[p.Name!]);
        return compiledExpression.Compile()?.DynamicInvoke(args.ToArray());
    }

    /// <summary>
    /// Executes sorted sequence of named expressions. 
    /// </summary>
    /// <remarks>Potentially will be able to use values of already executed expressions as arguments for following ones</remarks>
    /// <param name="compiledExpressions">Sorted sequence of expressions and their names: (name1, expression1), (name2, expression2)... </param>
    /// <param name="arguments">Dictionary of argument values for the expressions</param>
    /// <returns>Sequence of calculated expressions and their names: (name1, value1), (name2, value2)...</returns>
    public static IEnumerable<(TExpressionProperties, object?)> Execute<TExpressionProperties>(
        this IEnumerable<CompiledExpression<TExpressionProperties>> compiledExpressions,
        IDictionary<string, object?> arguments)
        where TExpressionProperties : IExpressionProperties
    {
        var allArguments = new Dictionary<string, object?>(arguments);
        return compiledExpressions.Select(e =>
        {
            var result = Execute(e, allArguments);
            allArguments[e.Properties.Name] = result;
            return (e.Properties, result);
        });
    }
}
