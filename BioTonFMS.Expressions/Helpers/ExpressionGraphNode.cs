using BioTonFMS.Expressions.Ast;
using BioTonFMS.Expressions.Util;

namespace BioTonFMS.Expressions;

/// <summary>
/// Узел графа выражений
/// </summary>
/// <typeparam name="TExpressionProperties"></typeparam>
public struct ExpressionGraphNode<TExpressionProperties> : IGraphNode
    where TExpressionProperties : IExpressionProperties
{
    /// <summary>
    /// Конструирует узел графа выражений
    /// </summary>
    /// <param name="edges">Грани исходящие из данного узла к другим узлам</param>
    /// <param name="ast">AST представляющее выражение этого узла</param>
    /// <param name="properties">Свойства выражения этого узла</param>
    public ExpressionGraphNode(string[] edges, AstNode? ast, TExpressionProperties properties)
    {
        Edges = edges;
        Ast = ast;
        Properties = properties;
    }

    /// <summary>
    /// Грани исходящие из данного узла к другим узлам
    /// </summary>
    public string[] Edges { get; }
    
    /// <summary>
    /// AST представляющее выражение этого узла
    /// </summary>
    public AstNode? Ast { get; }
    
    /// <summary>
    /// Свойства выражения этого узла
    /// </summary>
    public TExpressionProperties Properties { get; }
}
