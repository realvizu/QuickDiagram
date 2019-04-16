using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Codartis.SoftVis.Graphs.Concurrent;
using QuickGraph;
using Xunit;

namespace Codartis.SoftVis.UnitTests.Graphs.Concurrent
{
    public class ConcurrentBidirectionalGraphTests
    {
        [Fact]
        public void IsThreadSafe()
        {
            var graph = new ConcurrentBidirectionalGraph<int, Edge<int>>();
            const int repeat = 1000;

            var tasks = new List<Task>
            {
                Task.Run(() => ReadAndWriteGraph(graph, repeat, 1)),
                Task.Run(() => ReadAndWriteGraph(graph, repeat, 2)),
                Task.Run(() => ReadAndWriteGraph(graph, repeat, 3)),
            };

            Task.WaitAll(tasks.ToArray());
        }

        private static void ReadAndWriteGraph(ConcurrentBidirectionalGraph<int, Edge<int>> graph, int repeat, int instance)
        {
            var vertex1 = instance;
            graph.AddVertex(vertex1);

            for (int i = 0; i < repeat; i++)
            {
                var vertex2 = i + repeat * instance;
                graph.AddVertex(vertex2);
                Debug.WriteLine($"Vertices={graph.VertexCount}");

                graph.AddEdge(new Edge<int>(vertex1, vertex2));
                Debug.WriteLine($"Edges={graph.EdgeCount}");
            }
        }
    }
}
