using System.Collections.Generic;
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

        public IDictionary<TEdge, Route> EdgeRoutes { get; private set; }

        internal StraightEdgeRoutingAlgorithm(IEdgeSet<TVertex, TEdge> graph)
        {
            _graph = graph;
        }

        public void Compute()
        {
            var routes = new Dictionary<TEdge, Route>();

            foreach (var edge in _graph.Edges)
            {
                var route = CreateRoute(edge);
                routes.Add(edge, route);
            }

            EdgeRoutes = routes;
        }

        private static Route CreateRoute(TEdge edge)
        {
            var sourceRect = edge.Source.Rect;
            var targetRect = edge.Target.Rect;

            return new Route
            {
                sourceRect.GetAttachPointToward(targetRect.Center),
                targetRect.GetAttachPointToward(sourceRect.Center)
            };
        }
    }
}
