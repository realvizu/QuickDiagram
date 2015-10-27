using System;
using System.Collections.Generic;
using Codartis.SoftVis.Diagramming.Layout.ActionTracking;
using Codartis.SoftVis.Diagramming.Layout.ActionTracking.Implementation;
using Codartis.SoftVis.Geometry;

namespace Codartis.SoftVis.Diagramming.Layout.Incremental
{
    /// <summary>
    /// Calculates positions whenever vertices and edges are added.
    /// Sends events about vertex position and edge route changes.
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
    /// Events:
    /// <para>The layout engine subscribes to the vertices center property changed event.</para>
    /// <para>DiagramNodeCenterChanged event is fired whenever a vertex is moved.</para>
    /// <para>DiagramConnectorRouteChanged event is fired when the edge is first added and whenever any of the end-vertices are moved.</para>
    /// </remarks>
    internal sealed class IncrementalLayoutEngine2
    {
        private readonly double _horizontalGap;
        private readonly double _verticalGap;
        private readonly LayeringGraph _layeringGraph;
        private readonly Dictionary<DiagramNode, LayeringVertex> _diagramNodeToLayeringVertexMap;
        private readonly Dictionary<DiagramConnector, LayeringEdge> _diagramConnectorToLayeringEdgeMap;

        public int TotalVertexMoveCount { get; private set; }
        public ILayoutActionGraph LastLayoutActionGraph { get; private set; }

        public event EventHandler<RectMove> DiagramNodeCenterChanged;
        public event EventHandler<Route> DiagramConnectorRouteChanged;

        public IncrementalLayoutEngine2(double horizontalGap, double verticalGap)
        {
            _horizontalGap = horizontalGap;
            _verticalGap = verticalGap;
            _layeringGraph = new LayeringGraph();
            _diagramNodeToLayeringVertexMap = new Dictionary<DiagramNode, LayeringVertex>();
            _diagramConnectorToLayeringEdgeMap = new Dictionary<DiagramConnector, LayeringEdge>();

            LastLayoutActionGraph = new LayoutActionGraph();
        }

        public void Clear()
        {
            _layeringGraph.Clear();
            _diagramNodeToLayeringVertexMap.Clear();
            _diagramConnectorToLayeringEdgeMap.Clear();
            TotalVertexMoveCount = 0;
        }

        public void Add(DiagramNode diagramNode)
        {
            var layerAwareVertex = new LayeringVertex(diagramNode);
            _layeringGraph.AddVertex(layerAwareVertex);
            _diagramNodeToLayeringVertexMap.Add(diagramNode, layerAwareVertex);
        }

        public void Remove(DiagramNode diagramNode)
        {
            var layerAwareVertex = _diagramNodeToLayeringVertexMap[diagramNode];
            _layeringGraph.RemoveVertex(layerAwareVertex);
            _diagramNodeToLayeringVertexMap.Remove(diagramNode);
        }

        public void Add(DiagramConnector diagramConnector)
        {
            var source = _diagramNodeToLayeringVertexMap[diagramConnector.Source];
            var target = _diagramNodeToLayeringVertexMap[diagramConnector.Target];

            var layeringGraphEdge = new LayeringEdge(source, target);
            _layeringGraph.AddEdge(layeringGraphEdge);
            _diagramConnectorToLayeringEdgeMap.Add(diagramConnector, layeringGraphEdge);
        }

        public void Remove(DiagramConnector diagramConnector)
        {
            var layeringGraphEdge = _diagramConnectorToLayeringEdgeMap[diagramConnector];
            _layeringGraph.RemoveEdge(layeringGraphEdge);
            _diagramConnectorToLayeringEdgeMap.Remove(diagramConnector);
        }

    }
}
