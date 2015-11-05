using System;
using Codartis.SoftVis.Common;
using Codartis.SoftVis.Diagramming.Layout.ActionTracking;
using Codartis.SoftVis.Diagramming.Layout.Incremental.ActionTracking;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Graphs;

namespace Codartis.SoftVis.Diagramming.Layout.Incremental
{
    /// <summary>
    /// Attaches to a DiagramGraph's events and calculates and publishes layout action events
    /// whenever vertices and edges are added or removed.
    /// </summary>
    /// <remarks>
    /// Layout rules:
    /// <para>Adding a new node adds it to the first layer in node name order.</para>
    /// <para>Adding a new inheritance connection moves the source node under the target node.
    /// The source node brings all its children with it.</para>
    /// <para>If the source node has siblings then it is placed among them based on node name order.</para>
    /// <para>If the source node has no siblings then it is placed between the children of the parent's preceding and following nodes.
    /// It ensures that there won't be any inheritance edge crossings.</para>
    /// <para>The ordering of the nodes' horizontal position always correspond to their order in the layer. 
    /// To maintain this correspondance some nodes can be pushed to the left or right when placing a new node.</para>
    /// <para>Pushing away nodes also ensures that there are no overlapping nodes.</para>
    /// <para>If a node is pushed then all its descendants are pushed with it.</para>
    /// <para>When a tree is moved it is first set to "floating", that is it won't cause any vertex overlaps.
    /// (To avoid collision with itself.)</para>
    /// <para>When a vertex is placed (its center property is set) then it stops floating.</para>
    /// <para>When a vertex is placed (for any reason) its vertical position is acquired from its layer.</para>
    /// <para>When a vertex is pushed then its parent is centered again to its child block (recursive on parents upwards),
    /// possibly pushing away other nodes.</para>
    /// <para>When a vertex is moved then all of its connectors' routes are recalculated.</para>
    /// <para>At the end of each layout change the diagram is compacted if possible. (Unnecessary spaces removed.)</para>
    /// </remarks>
    internal sealed class IncrementalLayoutEngine : IncrementalLayoutActionEventSource
    {
        private readonly DiagramGraph _diagramGraph;

        private readonly Map<DiagramNode, DiagramNodeLayoutVertex> _diagramNodeToLayoutVertexMap;
        private readonly Map<DiagramConnector, LayoutPath> _diagramConnectorToLayoutPathMap;
        private readonly Map<LayoutPath, Route> _layoutPathToPreviousRouteMap;
        private readonly LayoutGraph _layoutGraph;
        private readonly LayoutVertexLayers _layers;

        private readonly RelativeLayoutCalculator _relativeLayoutCalculator;
        private readonly AbsoluteLayoutCalculator _absoluteLayoutCalculator;

        public IncrementalLayoutEngine(DiagramGraph diagramGraph)
        {
            const double horizontalGap = DiagramDefaults.HorizontalGap;
            const double verticalGap = DiagramDefaults.VerticalGap;

            _diagramGraph = diagramGraph;

            _diagramNodeToLayoutVertexMap = new Map<DiagramNode, DiagramNodeLayoutVertex>();
            _diagramConnectorToLayoutPathMap = new Map<DiagramConnector, LayoutPath>();
            _layoutPathToPreviousRouteMap = new Map<LayoutPath, Route>();
            _layoutGraph = new LayoutGraph();
            _layers = new LayoutVertexLayers(_layoutGraph);

            _relativeLayoutCalculator = new RelativeLayoutCalculator(_diagramGraph, _layoutGraph, _layers);
            _relativeLayoutCalculator.LayoutActionExecuted += OnLayoutActionExecuted;

            _absoluteLayoutCalculator = new AbsoluteLayoutCalculator(_diagramGraph, _layoutGraph, _layers, horizontalGap, verticalGap);
            _absoluteLayoutCalculator.LayoutActionExecuted += OnLayoutActionExecuted;

            HookIntoDiagramGraphEvents();
        }

        private void HookIntoDiagramGraphEvents()
        {
            _diagramGraph.Cleared += OnDiagramGraphCleared;
            _diagramGraph.VertexAdded += OnDiagramNodeAdded;
            _diagramGraph.VertexRemoved += OnDiagramNodeRemoved;
            _diagramGraph.EdgeAdded += OnDiagramConnectorAdded;
            _diagramGraph.EdgeRemoved += OnDiagramConnectorRemoved;
        }

        private void OnDiagramGraphCleared(object sender, EventArgs e)
        {
            _relativeLayoutCalculator.Clear();
            _absoluteLayoutCalculator.Clear();

            _diagramNodeToLayoutVertexMap.Clear();
            _diagramConnectorToLayoutPathMap.Clear();
            _layoutPathToPreviousRouteMap.Clear();
            _layers.Clear();
            _layoutGraph.Clear();
        }

        private void OnDiagramNodeAdded(DiagramNode diagramNode)
        {
            RaiseDiagramNodeLayoutAction("AddDiagramNode", diagramNode);

            var diagramNodeVertex = new DiagramNodeLayoutVertex(diagramNode);
            _diagramNodeToLayoutVertexMap.Set(diagramNode, diagramNodeVertex);

            _relativeLayoutCalculator.Add(diagramNodeVertex);
            _absoluteLayoutCalculator.Add(diagramNodeVertex);
        }

        private void OnDiagramNodeRemoved(DiagramNode diagramNode)
        {
            RaiseDiagramNodeLayoutAction("RemoveDiagramNode", diagramNode);

            var layoutVertex = _diagramNodeToLayoutVertexMap.Get(diagramNode);

            // TODO: float->abs->rel ?
            _relativeLayoutCalculator.Remove(layoutVertex);
            _absoluteLayoutCalculator.Remove(layoutVertex);

            _diagramNodeToLayoutVertexMap.Remove(diagramNode);
        }

        private void OnDiagramConnectorAdded(DiagramConnector diagramConnector)
        {
            var layoutAction = RaiseDiagramConnectorLayoutAction("AddDiagramConnector", diagramConnector);

            var sourceVertex = _diagramNodeToLayoutVertexMap.Get(diagramConnector.Source);
            var targetVertex = _diagramNodeToLayoutVertexMap.Get(diagramConnector.Target);
            var newEdge = new LayoutEdge(sourceVertex, targetVertex, diagramConnector);
            var layoutPath = new LayoutPath(newEdge);
            _diagramConnectorToLayoutPathMap.Set(diagramConnector, layoutPath);

            _relativeLayoutCalculator.Add(layoutPath);
            _absoluteLayoutCalculator.Add(layoutPath);

            ReroutePath(layoutPath, layoutAction);
        }

        private void OnDiagramConnectorRemoved(DiagramConnector diagramConnector)
        {
            RaiseDiagramConnectorLayoutAction("RemoveDiagramConnector", diagramConnector);

            var layoutPath = _diagramConnectorToLayoutPathMap.Get(diagramConnector);

            // TODO: float->abs->rel ?
            _relativeLayoutCalculator.Remove(layoutPath);
            _absoluteLayoutCalculator.Remove(layoutPath);

            _diagramConnectorToLayoutPathMap.Remove(diagramConnector);
        }

        private void OnLayoutActionExecuted(object sender, ILayoutAction layoutAction)
        {
            RaiseLayoutAction(sender, layoutAction);

            var vertexMoveAction = layoutAction as IMoveVertexAction;
            if (vertexMoveAction == null)
                return;

            foreach (var edge in _layoutGraph.GetAllEdges(vertexMoveAction.Vertex))
            {
                var path = _diagramConnectorToLayoutPathMap.Get(edge.DiagramConnector);
                ReroutePath(path, layoutAction);
            }
        }

        private void ReroutePath(LayoutPath path, ILayoutAction causingAction)
        {
            if (path.IsFloating)
                return;

            var oldRoute = _layoutPathToPreviousRouteMap.Get(path);
            var newRoute = path.GetRoute();
            if (oldRoute == newRoute)
                return;

            _layoutPathToPreviousRouteMap.Set(path, newRoute);
            RaisePathLayoutAction("Reroute", path, oldRoute, newRoute, causingAction);
        }
    }
}
