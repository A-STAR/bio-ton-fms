using System.Linq.Expressions;
using BioTonFMS.Expressions.Ast;
using BioTonFMS.Expressions.Compilation;
using BioTonFMS.Expressions.Parsing;
using BioTonFMS.Expressions.Util;

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
    /// <param name="compiler">Compiler to use for compilation</param>
    /// <param name="exceptionHandler">Object which handles exception thrown during parsing, compiling or execution.</param>
    /// <returns>Expression tree which is the result of compilation of AST</returns>
    private static LambdaExpression? CompileWithHandler(this AstNode node, Compiler compiler, IExceptionHandler exceptionHandler)
    {
        try
        {
            return compiler.Compile(node);
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
    ///     Expressions may reference other expressions by their names</param>
    /// <param name="parameters">Types of parameters of the expressions by name</param>
    /// <param name="postprocess">Allows to change AST before compilation</param>
    /// <param name="compilationOptions"></param>
    /// <param name="exceptionHandler">Object which handles exception thrown during parsing, compiling or execution.</param>
    /// <returns>Set of compiled expressions with their names: (name1, expression1), (name2, expression2),...</returns>
    public static IEnumerable<CompiledExpression<TExpressionProperties>> SortAndBuild<TExpressionProperties>(
        this IEnumerable<TExpressionProperties> expressions,
        IDictionary<string, Type> parameters,
        Func<AstNode?, TExpressionProperties, AstNode?> postprocess,
        CompilationOptions? compilationOptions = null,
        IExceptionHandler? exceptionHandler = null)
        where TExpressionProperties : IExpressionProperties
    {
        exceptionHandler ??= new DefaultExceptionHandler();

        var graph = BuildExpressionGraph(expressions, exceptionHandler, postprocess);

        var allParameters = new Dictionary<string, Type>(parameters);

        var sortedExpressionNames = graph.TopologicalSort();
        sortedExpressionNames.Reverse();
        return sortedExpressionNames
            .Select(name =>
            {
                var node = graph[name];
                var compiler = new Compiler(allParameters, compilationOptions, node.Properties);
                var compiledExpression = node.Ast?.CompileWithHandler(compiler, exceptionHandler);
                if (compiledExpression is not null) allParameters.Add(name, compiledExpression.ReturnType);
                return new CompiledExpression<TExpressionProperties>(node.Properties, compiledExpression);
            });
    }
    public static Graph<ExpressionGraphNode<TExpressionProperties>> BuildExpressionGraph<TExpressionProperties>(
        IEnumerable<TExpressionProperties> expressions,
        IExceptionHandler? exceptionHandler = null, Func<AstNode?, TExpressionProperties, AstNode?>? postprocess = null)
        where TExpressionProperties : IExpressionProperties
    {
        var graph = expressions
            .ToDictionary(
                keySelector: e => e.Name,
                elementSelector: e =>
                {
                    var ast = ParseWithHandler(e.Formula, exceptionHandler ?? new DefaultExceptionHandler());
                    var postProcessedAst = postprocess == null ? ast : postprocess(ast, e);
                    return new ExpressionGraphNode<TExpressionProperties>(
                        postProcessedAst is null
                            ? Array.Empty<string>()
                            : postProcessedAst.GetVariables().Select(v => v.Name).ToArray(),
                        postProcessedAst, e);
                });
        return new Graph<ExpressionGraphNode<TExpressionProperties>>(graph);
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
        var args = compiledExpression.ExpressionTree.Parameters
            .Select(p => arguments.TryGetValue(p.Name!, out var value) ? value : null);
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
