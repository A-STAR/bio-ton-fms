namespace BioTonFMS.Expressions.Util;

public static class GraphUtil
{
    /// <summary>
    /// Sorts graph nodes topologically
    /// </summary>
    /// <remarks>Graph may contain loops. In this case algorithm doesn't add nodes reachable from looped nodes to the result</remarks>
    /// <param name="graph">Directed graph represented by dictionary where every key is a node and value is a list of outgoing edges.
    /// Lists of edges may contain references to nonexistent nodes in this case those references should be ignored</param>
    /// <returns>Topologically sorted list of graph nodes</returns>
    public static List<string> TopologicalSort<TGraphNode>(this Graph<TGraphNode> graph)
        where TGraphNode : IGraphNode
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
    
    public static List<string> FindAnyLoop<TGraphNode>(this Graph<TGraphNode> graph,
        string startingNode)
        where TGraphNode : IGraphNode
    {
        // Implements depth first search algorithm
        
        var namesStack = new List<string>();
        if (graph.Count == 0)
            return namesStack; 

        var indexStack = new List<int>();
        var nameSet = new HashSet<string>();
        nameSet.Add(startingNode);
        namesStack.Add(startingNode);
        indexStack.Add(0);
        while (namesStack.Count != 0)
        {
            var edges = graph[namesStack[^1]].Edges;
            var i = indexStack[^1];
            if (i == edges.Length)
            {
                // We are in Limbo. Go up
                var indexToRemove = indexStack.Count - 1;
                var nameToRemove = namesStack[indexToRemove];
                indexStack.RemoveAt(indexToRemove);
                namesStack.RemoveAt(indexToRemove);
                nameSet.Remove(nameToRemove);
                continue;
            }

            var nameToAdd = edges[i];
            if (!nameSet.Contains(nameToAdd))
            {
                // We need to go deeper
                indexStack[^1] += 1;
                namesStack.Add(nameToAdd);
                indexStack.Add(0);
                nameSet.Add(nameToAdd);
            }
            else
            {
                // Found cycle
                if (nameToAdd == startingNode)
                {
                    // It is cycle in which the starting node is a participant. Return the result
                    return namesStack;
                }
                else
                {
                    // It is cycle in which the starting node doesn't participate. Skip the loop.
                    indexStack[^1] += 1;
                }
            }
        }
        return namesStack;
    }
}