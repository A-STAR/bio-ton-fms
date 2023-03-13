using BioTonFMS.Expressions.Util;

namespace BiotonFMS.Telematica.Tests.Expressions;

public readonly struct GraphNodeMock : IGraphNode
{
    public GraphNodeMock(params string[] edges)
    {
        Edges = edges;
    }
    public string[] Edges { get; }

    public override string ToString()
    {
        return "[" + string.Join(", ", Edges) + "]";
    }
}
