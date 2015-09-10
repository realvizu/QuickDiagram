using System;
using System.Collections.Generic;
using System.Linq;

namespace Codartis.SoftVis.Diagramming.Shapes.Graph.Routing
{
    /// <summary>
    /// Implements an edge routing algorithm that simply connects the nodes with straight lines.
    /// </summary>
    internal class StraightRoutingAlgorithm
    {
        private readonly DiagramGraph _graph;

        internal StraightRoutingAlgorithm(DiagramGraph graph)
        {
            _graph = graph;
        }

        public IDictionary<DiagramConnector, DiagramPoint[]> ComputeEdgeRoutes()
        {
            var routes = new Dictionary<DiagramConnector, DiagramPoint[]>();

            foreach (var diagramConnector in _graph.Edges)
            {
                var route = CreateRoute(diagramConnector);
                routes.Add(diagramConnector, route);
            }

            return routes;
        }

        private static DiagramPoint[] CreateRoute(DiagramConnector diagramConnector)
        {
            var sourceRect = diagramConnector.Source.Rect;
            var targetRect = diagramConnector.Target.Rect;

            return AlignEndpointsToNodeSides(sourceRect, targetRect).ToArray();
        }

        private static IEnumerable<DiagramPoint> AlignEndpointsToNodeSides(DiagramRect sourceRect, DiagramRect targetRect)
        {
            yield return CalculateAttachPoint(sourceRect.Center, targetRect.Center, sourceRect.Size);
            yield return CalculateAttachPoint(targetRect.Center, sourceRect.Center, targetRect.Size);
        }

        private static DiagramPoint CalculateAttachPoint(DiagramPoint sourcePoint, DiagramPoint targetPoint, DiagramSize sourceNodeSize)
        {
            var sides = new double[4];
            sides[0] = (sourcePoint.X - sourceNodeSize.Width / 2.0 - targetPoint.X) / (sourcePoint.X - targetPoint.X);
            sides[1] = (sourcePoint.Y - sourceNodeSize.Height / 2.0 - targetPoint.Y) / (sourcePoint.Y - targetPoint.Y);
            sides[2] = (sourcePoint.X + sourceNodeSize.Width / 2.0 - targetPoint.X) / (sourcePoint.X - targetPoint.X);
            sides[3] = (sourcePoint.Y + sourceNodeSize.Height / 2.0 - targetPoint.Y) / (sourcePoint.Y - targetPoint.Y);

            double fi = 0;
            for (var i = 0; i < 4; i++)
            {
                if (sides[i] <= 1)
                    fi = Math.Max(fi, sides[i]);
            }

            return targetPoint + fi * (sourcePoint - targetPoint);
        }
    }
}
