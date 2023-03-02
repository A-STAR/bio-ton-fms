namespace BioTonFMS.Expressions;

public static class Util
{
    /// <summary>
    /// Sorts graph nodes topologically
    /// </summary>
    /// <remarks>Graph may contain loops. In this case algorithm doesn't add nodes reachable from looped nodes to the result</remarks>
    /// <param name="graph">Directed graph represented by dictionary where every key is a node and value is a list of outgoing edges.
    /// Lists of edges may contain references to nonexistent nodes in this case those references should be ignored</param>
    /// <returns>Topologically sorted list of graph nodes</returns>
    public static List<string> TopologicalSort<TData>(this IDictionary<string, (ICollection<string> Edges, TData Data)> graph)
    {
        var inDegrees = new Dictionary<string, int>();
        foreach (var node in graph.Keys)
        {
            inDegrees[node] = 0;
        }

        foreach (var node in graph.Values)
        {
            foreach (var nodeName in node.Edges)
            {
                if (inDegrees.ContainsKey(nodeName))
                    inDegrees[nodeName]++;
            }
        }

        var queue = new Queue<string>();
        foreach (var node in inDegrees.Keys.Where(node => inDegrees[node] == 0))
        {
            queue.Enqueue(node);
        }

        var sorted = new List<string>();
        while (queue.Count > 0)
        {
            var node = queue.Dequeue();
            sorted.Add(node);

            foreach (var adjNode in graph[node].Edges)
            {
                if (!inDegrees.ContainsKey(adjNode))
                    continue;

                inDegrees[adjNode]--;

                if (inDegrees[adjNode] == 0)
                {
                    queue.Enqueue(adjNode);
                }
            }
        }
        return sorted;
    }
}
