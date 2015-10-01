using Codartis.SoftVis.Diagramming.Graph.Layout.VertexPlacement.Incremental;
using QuickGraph;

namespace Codartis.SoftVis.Diagramming.Graph
{
    /// <summary>
    /// The graph formed by the nodes and connectors of a diagram.
    /// </summary>
    internal sealed class DiagramGraph : BidirectionalGraph<DiagramNode, DiagramConnector>
    {
        private const double HorizontalGap = 20;
        private const double VerticalGap = 40;

        private readonly IncrementalLayoutEngine _layoutEngine;

        public DiagramGraph()
        {
            _layoutEngine = new IncrementalLayoutEngine(HorizontalGap, VerticalGap);
            _layoutEngine.VertexCenterChanged += OnVertexCenterChanged;
            _layoutEngine.EdgeRouteChanged += OnEdgeRouteChanged;
        }

        public override bool AddVertex(DiagramNode node)
        {
            var isAdded = base.AddVertex(node);

            if (isAdded)
                _layoutEngine.Add(node);

            return isAdded;
        }

        public override bool AddEdge(DiagramConnector connector)
        {
            var isAdded = base.AddEdge(connector);

            if (isAdded)
                _layoutEngine.Add(connector);

            return isAdded;
        }

        private static void OnVertexCenterChanged(object sender, DiagramPoint diagramPoint)
        {
            ((DiagramNode)sender).Center = diagramPoint;
        }

        private static void OnEdgeRouteChanged(object sender, DiagramPoint[] routePoints)
        {
            ((DiagramConnector)sender).RoutePoints = routePoints;
        }
    }
}
