using BioTonFMS.Expressions.Util;
using FluentAssertions;

namespace BiotonFMS.Telematica.Tests.Expressions;

public class GraphUtilTests
{
    public static KeyValuePair<string, GraphNodeMock> GraphNode(string name, params string[] edges) => new(name, new GraphNodeMock(edges));
    public static (string Start, string[] Cycle) Cycle(string start, params string[] cycle) => (start, cycle);

    private static readonly (Graph<GraphNodeMock> Graph, (string Start, string[] Cycle)[] Cycles)[] TestGraphs =
    {
        ( // Empty
            Graph: new Graph<GraphNodeMock>(new Dictionary<string, GraphNodeMock>()),
            Cycles: new[]
            {
                (Start: "", Cycle: Array.Empty<string>())
            }
        ),
        ( // Single unconnected node
            Graph: new Graph<GraphNodeMock>(new Dictionary<string, GraphNodeMock>(new[]
            {
                GraphNode("a")
            })),
            Cycles: new[]
            {
                Cycle("a")
            }
        ),
        ( // Self cycle
            Graph: new Graph<GraphNodeMock>(new Dictionary<string, GraphNodeMock>(new[]
            {
                GraphNode("a", "a")
            })),
            Cycles: new[]
            {
                Cycle("a", "a")
            }
        ),
        ( // Lateral self cycle
            Graph: new Graph<GraphNodeMock>(new Dictionary<string, GraphNodeMock>(new[]
            {
                GraphNode("a", "b"),
                GraphNode("b", "b"),
            })),
            Cycles: new[]
            {
                Cycle("a")
            }
        ),
        ( // Two node cycle
            Graph: new Graph<GraphNodeMock>(new Dictionary<string, GraphNodeMock>(new[]
            {
                GraphNode("a", "b"),
                GraphNode("b", "a"),
            })),
            Cycles: new[]
            {
                Cycle("a", "a", "b"),
                Cycle("b", "b", "a"),
            }
        ),
        ( // Two cycles
            Graph: new Graph<GraphNodeMock>(new Dictionary<string, GraphNodeMock>(new[]
            {
                GraphNode("a", "b", "c"),
                GraphNode("b", "a"),
                GraphNode("c", "a"),
            })),
            Cycles: new[]
            {
                Cycle("a", "a", "b"),
                Cycle("b", "b", "a"),
                Cycle("c", "c", "a"),
            }
        ),
        ( // Hairy cycle
            Graph: new Graph<GraphNodeMock>(new Dictionary<string, GraphNodeMock>(new[]
            {
                GraphNode("a", "b"),
                GraphNode("b", "c"),
                GraphNode("c", "b", "d"),
                GraphNode("d"),
            })),
            Cycles: new[]
            {
                Cycle("a"),
                Cycle("b", "b", "c"),
                Cycle("c", "c", "b"),
                Cycle("d"),
            }
        ),
    };

    public static IEnumerable<object[]> CyclesTestSamples
    {
        get
        {
            foreach (var graph in TestGraphs)
            {
                foreach (var cycle in graph.Cycles)
                {
                    var arguments = new object[3];
                    arguments[0] = graph.Graph;
                    arguments[1] = cycle.Start;
                    arguments[2] = cycle.Cycle;
                    yield return arguments;
                }
            }
        }
    }

    [Theory, MemberData(nameof(CyclesTestSamples))]
    public void FindAnyCycle(Graph<GraphNodeMock> graph, string startingNode, string[] referenceCycle)
    {
        var cycle = graph.FindAnyLoop(startingNode);

        cycle.Should().BeEquivalentTo(referenceCycle);
    }
}
