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

        private readonly Map<LayoutPath, Route> _layoutPathToPreviousRouteMap;

        private readonly RelativeLayoutCalculator _relativeLayoutCalculator;
        private readonly AbsoluteLayoutCalculator _absoluteLayoutCalculator;
        private readonly RelativeLayoutActionDispatcherVisitor _relativeLayoutActionDispatcher;

        public IncrementalLayoutEngine(DiagramGraph diagramGraph)
        {
            const double horizontalGap = DiagramDefaults.HorizontalGap;
            const double verticalGap = DiagramDefaults.VerticalGap;

            _diagramGraph = diagramGraph;

            _layoutPathToPreviousRouteMap = new Map<LayoutPath, Route>();

            _relativeLayoutCalculator = new RelativeLayoutCalculator();
            _relativeLayoutCalculator.LayoutActionExecuted += OnLayoutActionExecuted;

            _absoluteLayoutCalculator = new AbsoluteLayoutCalculator(_relativeLayoutCalculator, horizontalGap, verticalGap);
            _absoluteLayoutCalculator.LayoutActionExecuted += OnLayoutActionExecuted;

            _relativeLayoutActionDispatcher = new RelativeLayoutActionDispatcherVisitor(_absoluteLayoutCalculator);
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
            _relativeLayoutCalculator.OnDiagramCleared();
            _absoluteLayoutCalculator.OnLayoutCleared();

            _layoutPathToPreviousRouteMap.Clear();
        }

        private void OnDiagramNodeAdded(DiagramNode diagramNode)
        {
            var layoutAction = RaiseDiagramNodeLayoutAction("AddDiagramNode", diagramNode);
            _relativeLayoutCalculator.OnDiagramNodeAdded(diagramNode, layoutAction);
        }

        private void OnDiagramNodeRemoved(DiagramNode diagramNode)
        {
            var layoutAction = RaiseDiagramNodeLayoutAction("RemoveDiagramNode", diagramNode);
            // TODO: float->abs->rel ?
            _relativeLayoutCalculator.OnDiagramNodeRemoved(diagramNode, layoutAction);
        }

        private void OnDiagramConnectorAdded(DiagramConnector diagramConnector)
        {
            var layoutAction = RaiseDiagramConnectorLayoutAction("AddDiagramConnector", diagramConnector);
            _relativeLayoutCalculator.OnDiagramConnectorAdded(diagramConnector, layoutAction);
        }

        private void OnDiagramConnectorRemoved(DiagramConnector diagramConnector)
        {
            var layoutAction = RaiseDiagramConnectorLayoutAction("RemoveDiagramConnector", diagramConnector);
            // TODO: float->abs->rel ?
            _relativeLayoutCalculator.OnDiagramConnectorRemoved(diagramConnector, layoutAction);
        }

        private void OnLayoutActionExecuted(object sender, ILayoutAction layoutAction)
        {
            var relativeLayoutAction = layoutAction as IRelativeLayoutAction;
            relativeLayoutAction?.AcceptVisitor(_relativeLayoutActionDispatcher);

            RaiseLayoutAction(sender, layoutAction);
        }
    }
}
