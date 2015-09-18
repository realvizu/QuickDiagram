using System.Collections.Generic;
using System.Linq;
using QuickGraph;

namespace Codartis.SoftVis.Diagramming.Graph.Layout
{
    /// <summary>
    /// Implements an edge routing algorithm that simply connects the nodes with straight lines.
    /// </summary>
    internal class StraightEdgeRoutingAlgorithm<TVertex, TEdge> : IEdgeRoutingAlgorithm<TVertex, TEdge>
        where TVertex : IPositionedExtent
        where TEdge : IEdge<TVertex>
    {
        private readonly IEdgeSet<TVertex, TEdge> _graph;

        public IDictionary<TEdge, DiagramPoint[]> EdgeRoutes { get; private set; }

        internal StraightEdgeRoutingAlgorithm(IEdgeSet<TVertex, TEdge> graph)
        {
            _graph = graph;
        }

        public void Compute()
        {
            var routes = new Dictionary<TEdge, DiagramPoint[]>();

            foreach (var edge in _graph.Edges)
            {
                var route = CreateRoute(edge);
                routes.Add(edge, route);
            }

            EdgeRoutes = routes;
        }

        private static DiagramPoint[] CreateRoute(TEdge edge)
        {
            var sourceRect = edge.Source.Rect;
            var targetRect = edge.Target.Rect;

            return AlignEndpointsToNodeSides(sourceRect, targetRect).ToArray();
        }

        private static IEnumerable<DiagramPoint> AlignEndpointsToNodeSides(DiagramRect sourceRect, DiagramRect targetRect)
        {
            yield return sourceRect.GetAttachPointToward(targetRect.Center);
            yield return targetRect.GetAttachPointToward(sourceRect.Center);
        }
    }
}
