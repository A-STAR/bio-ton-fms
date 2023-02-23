using System.Linq.Expressions;
using BioTonFMS.Expressions.AST;

namespace BioTonFMS.Expressions;

public static class Helpers
{
    /// <summary>
    /// Sorts graph nodes topologically
    /// </summary>
    /// <remarks>Graph may contain loops. In this case algorithm doesn't add nodes reachable from looped nodes to the result</remarks>
    /// <param name="graph">Directed graph represented by dictionary where every key is a node and value is a list of outgoing edges.
    /// Lists of edges may contain references to nonexistent nodes in this case those references should be ignored</param>
    /// <returns>Topologically sorted list of graph nodes</returns>
    private static IEnumerable<string> TopologicalSort(this IDictionary<string, ICollection<string>> graph)
    {
        // Here we should do a topological sorting of expressions but since we do not have expressions which depend on each
        // other for the moment, it wasn't implemented.
        return graph.Keys;
    }

    /// <summary>
    /// Builds set of mutually dependent expressions
    /// </summary>
    /// <param name="expressions">Sequence of pairs of expressions and their names: (name1, expression1), (name2, expression2),...
    /// Expressions may reference other expressions by their names</param>
    /// <param name="parameters">Types of parameters of the expressions by name</param>
    /// <param name="executionHandler">Object which executes different parts of formula calculation process.
    /// May be used for exception handling.</param>
    /// <returns>Set of compiled expressions with their names: (name1, expression1), (name2, expression2),...</returns>
    public static IEnumerable<(string, Expression?)> SortAndBuild(
        this IEnumerable<(string Name, (string Formula, bool UseFallbacks) Descr)> expressions,
        IDictionary<string, Type> parameters, IExecutionHandler? executionHandler = null)
    {
        executionHandler ??= new DefaultExecutionHandler();

        var astByName = expressions.ToDictionary(e => e.Name,
            e => (Ast: Parser.ParseWithHandler(e.Descr.Formula, executionHandler.ExecuteParsing), e.Descr.UseFallbacks));

        var graph = astByName.ToDictionary<KeyValuePair<string, (AstNode? Ast, bool)>, string, ICollection<string>>(a => a.Key,
            a => a.Value.Ast is null ? Array.Empty<string>() : a.Value.Ast.GetVariables().ToArray());

        var allParameters = new Dictionary<string, Type>(parameters);

        return graph.TopologicalSort().Select(name =>
        {
            var astTuple = astByName[name];
            var compiler = new Compiler(new CompilerOptions
            {
                UseFallbacks = astTuple.UseFallbacks
            });
            var compiledExpression = astTuple.Ast?.CompileWithHandler(compiler, allParameters, executionHandler.ExecuteCompilation);
            if (compiledExpression is not null) allParameters.Add(name, compiledExpression.Type);
            return (p: name, compiledExpression);
        });
    }

    /// <summary>
    /// Executes lambda expression with specified arguments
    /// </summary>
    /// <param name="lambda">Compiled lambda expression. Has a set of named parameters which are bound to supplied arguments by name</param>
    /// <param name="arguments">Dictionary of named arguments for lambda. It may contain more arguments than required by the lambda</param>
    /// <returns>Result of execution of the lambda expression</returns>
    public static object? Execute(LambdaExpression lambda, IDictionary<string, object?> arguments)
    {
        var args = lambda.Parameters.Select(p => arguments[p.Name!]);
        return lambda.Compile().DynamicInvoke(args.ToArray());
    }

    /// <summary>
    /// Executes sorted sequence of named expressions. 
    /// </summary>
    /// <remarks>Potentially will be able to use values of already executed expressions as arguments for following ones</remarks>
    /// <param name="compiledExpressions">Sorted sequence of expressions and their names: (name1, expression1), (name2, expression2)... </param>
    /// <param name="arguments">Dictionary of argument values for the expressions</param>
    /// <returns>Sequence of calculated expressions and their names: (name1, value1), (name2, value2)...</returns>
    public static IEnumerable<(string, object?)> Execute(this IEnumerable<(string, Expression?)> compiledExpressions,
        IDictionary<string, object?> arguments)
    {
        return compiledExpressions.Select(e => (e.Item1, e.Item2 is LambdaExpression item2 ? Execute(item2, arguments) : null));
    }
}