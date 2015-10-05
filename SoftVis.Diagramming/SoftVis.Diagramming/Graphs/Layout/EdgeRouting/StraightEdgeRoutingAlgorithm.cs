using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Geometry;
using QuickGraph;

namespace Codartis.SoftVis.Graphs.Layout.EdgeRouting
{
    /// <summary>
    /// Implements an edge routing algorithm that simply connects the nodes with straight lines.
    /// </summary>
    internal class StraightEdgeRoutingAlgorithm<TVertex, TEdge> : IEdgeRoutingAlgorithm<TVertex, TEdge>
        where TVertex : IRect
        where TEdge : IEdge<TVertex>
    {
        private readonly IEdgeSet<TVertex, TEdge> _graph;

        public IDictionary<TEdge, Point2D[]> EdgeRoutes { get; private set; }

        internal StraightEdgeRoutingAlgorithm(IEdgeSet<TVertex, TEdge> graph)
        {
            _graph = graph;
        }

        public void Compute()
        {
            var routes = new Dictionary<TEdge, Point2D[]>();

            foreach (var edge in _graph.Edges)
            {
                var route = CreateRoute(edge);
                routes.Add(edge, route);
            }

            EdgeRoutes = routes;
        }

        private static Point2D[] CreateRoute(TEdge edge)
        {
            var sourceRect = edge.Source.Rect;
            var targetRect = edge.Target.Rect;

            return AlignEndpointsToNodeSides(sourceRect, targetRect).ToArray();
        }

        private static IEnumerable<Point2D> AlignEndpointsToNodeSides(Rect2D sourceRect, Rect2D targetRect)
        {
            yield return sourceRect.GetAttachPointToward(targetRect.Center);
            yield return targetRect.GetAttachPointToward(sourceRect.Center);
        }
    }
}
