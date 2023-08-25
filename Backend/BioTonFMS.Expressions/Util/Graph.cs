namespace BioTonFMS.Expressions.Util;

public class Graph<TGraphNode> : Dictionary<string, TGraphNode>
    where TGraphNode : IGraphNode
{
    public Graph(IDictionary<string, TGraphNode> graph) : base(graph) { }
}
