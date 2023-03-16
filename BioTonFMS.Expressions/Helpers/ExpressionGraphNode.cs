using BioTonFMS.Expressions.Ast;
using BioTonFMS.Expressions.Util;

namespace BioTonFMS.Expressions;

public struct ExpressionGraphNode<TExpressionProperties> : IGraphNode
    where TExpressionProperties : IExpressionProperties
{
    public ExpressionGraphNode(string[] edges, AstNode? ast, TExpressionProperties properties)
    {
        Edges = edges;
        Ast = ast;
        Properties = properties;
    }

    public string[] Edges { get; }
    public AstNode? Ast { get; }
    public TExpressionProperties Properties { get; }
}
