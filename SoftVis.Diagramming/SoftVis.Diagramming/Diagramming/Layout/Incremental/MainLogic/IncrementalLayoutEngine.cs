using System;
using Codartis.SoftVis.Common;
using Codartis.SoftVis.Diagramming.Layout.BaseActions;
using Codartis.SoftVis.Diagramming.Layout.Incremental.Absolute.Logic;
using Codartis.SoftVis.Diagramming.Layout.Incremental.Relative;
using Codartis.SoftVis.Diagramming.Layout.Incremental.Relative.Logic;
using Codartis.SoftVis.Geometry;

namespace Codartis.SoftVis.Diagramming.Layout.Incremental.MainLogic
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
    internal sealed class IncrementalLayoutEngine : BaseLayoutActionEventSource
    {
        private readonly DiagramGraph _diagramGraph;

        private readonly Map<DiagramNode, DiagramNodeLayoutVertex> _diagramNodeToLayoutVertexMap;
        private readonly Map<DiagramConnector, LayoutPath> _diagramConnectorToLayoutPathMap;
        private readonly Map<LayoutPath, Route> _layoutPathToPreviousRouteMap;

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

            _relativeLayoutCalculator = new RelativeLayoutCalculator();
            _relativeLayoutCalculator.LayoutActionExecuted += OnLayoutActionExecuted;

            _absoluteLayoutCalculator = new AbsoluteLayoutCalculator(_relativeLayoutCalculator.RelativeLayout, horizontalGap, verticalGap);
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
            _absoluteLayoutCalculator.OnLayoutCleared();
            _relativeLayoutCalculator.OnDiagramCleared();

            _layoutPathToPreviousRouteMap.Clear();
            _diagramConnectorToLayoutPathMap.Clear();
            _diagramNodeToLayoutVertexMap.Clear();
        }

        private void OnDiagramNodeAdded(DiagramNode diagramNode)
        {
            if (_diagramNodeToLayoutVertexMap.ContainsKey(diagramNode))
                throw new InvalidOperationException($"Diagram node {diagramNode} already added.");

            var layoutAction = RaiseDiagramNodeLayoutAction("AddDiagramNode", diagramNode);

            var diagramNodeLayoutVertex = new DiagramNodeLayoutVertex(diagramNode);
            _diagramNodeToLayoutVertexMap.Set(diagramNode, diagramNodeLayoutVertex);

            _relativeLayoutCalculator.OnDiagramNodeAdded(diagramNodeLayoutVertex, layoutAction);
            _absoluteLayoutCalculator.OnDiagramNodeAdded(diagramNodeLayoutVertex, layoutAction);
        }

        private void OnDiagramNodeRemoved(DiagramNode diagramNode)
        {
            if (!_diagramNodeToLayoutVertexMap.ContainsKey(diagramNode))
                throw new InvalidOperationException($"Diagram node {diagramNode} not found.");

            var layoutAction = RaiseDiagramNodeLayoutAction("RemoveDiagramNode", diagramNode);

            var diagramNodeLayoutVertex = _diagramNodeToLayoutVertexMap.Get(diagramNode);
            _diagramNodeToLayoutVertexMap.Remove(diagramNode);

            _absoluteLayoutCalculator.OnDiagramNodeRemoved(diagramNodeLayoutVertex, layoutAction);
            _relativeLayoutCalculator.OnDiagramNodeRemoved(diagramNodeLayoutVertex, layoutAction);
        }

        private void OnDiagramConnectorAdded(DiagramConnector diagramConnector)
        {
            if (_diagramConnectorToLayoutPathMap.ContainsKey(diagramConnector))
                throw new InvalidOperationException($"Diagram connector {diagramConnector} already added.");

            var layoutAction = RaiseDiagramConnectorLayoutAction("AddDiagramConnector", diagramConnector);

            var layoutPath = CreateLayoutPath(diagramConnector);
            _diagramConnectorToLayoutPathMap.Set(diagramConnector, layoutPath);

            _relativeLayoutCalculator.OnDiagramConnectorAdded(layoutPath, layoutAction);
            _absoluteLayoutCalculator.OnDiagramConnectorAdded(layoutPath, layoutAction);
        }

        private void OnDiagramConnectorRemoved(DiagramConnector diagramConnector)
        {
            if (!_diagramConnectorToLayoutPathMap.ContainsKey(diagramConnector))
                throw new InvalidOperationException($"Diagram connector {diagramConnector} not found.");

            var layoutAction = RaiseDiagramConnectorLayoutAction("RemoveDiagramConnector", diagramConnector);

            var layoutPath = _diagramConnectorToLayoutPathMap.Get(diagramConnector);
            _diagramConnectorToLayoutPathMap.Remove(diagramConnector);

            _absoluteLayoutCalculator.OnDiagramConnectorRemoved(layoutPath, layoutAction);
            _relativeLayoutCalculator.OnDiagramConnectorRemoved(layoutPath, layoutAction);
        }

        private LayoutPath CreateLayoutPath(DiagramConnector diagramConnector)
        {
            var sourceVertex = _diagramNodeToLayoutVertexMap.Get(diagramConnector.Source);
            var targetVertex = _diagramNodeToLayoutVertexMap.Get(diagramConnector.Target);
            return new LayoutPath(sourceVertex, targetVertex, diagramConnector);
        }

        private void OnLayoutActionExecuted(object sender, ILayoutAction layoutAction)
        {
            RaiseLayoutAction(sender, layoutAction);
        }
    }
}
