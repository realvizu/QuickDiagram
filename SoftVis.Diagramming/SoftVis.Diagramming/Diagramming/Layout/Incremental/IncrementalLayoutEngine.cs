using System;
using System.Collections.Generic;
using System.Diagnostics;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Graphs;

namespace Codartis.SoftVis.Diagramming.Layout.Incremental
{
    /// <summary>
    /// Calculates positions whenever vertices and edges are added.
    /// Sends events about vertex position and edge route changes.
    /// </summary>
    /// <remarks>
    /// Layout rules:
    /// <para>Adding a new node adds it to the first layer, at the rightmost free position. No overlap can occur.</para>
    /// <para>Adding a new connection moves the source node under the target node. The source node brings all its children with it.</para>
    /// <para>If the source node has siblings then it is placed among them based on node ordering (by node name).</para>
    /// <para>If the newly placed node overlaps any existing then those are pushed to the left and right (based on relative center position).</para>
    /// <para>If the newly placed node pushes away an existing node then it pushes all its siblings as well (not to break a continuous sibling block).</para>
    /// <para>If a node is pushed then all its descendants are pushed with it.</para>
    /// <para>When a tree is moved it is first set to "floating", that is it won't cause any vertex overlaps.</para>
    /// <para>When a vertex is placed (its center property is set) then it stops floating.</para>
    /// <para>When a vertex is placed (for any reason) its vertical position is acquired from its layer.</para>
    /// <para>When a vertex is moved because of overlap resolution then its parent is centered again to its child block (recursive on parents upwards), possibly pushing away other nodes.</para>
    /// Events:
    /// <para>The layout engine subscribes to the vertices center property changed event.</para>
    /// <para>VertexCenterChanged event is fired whenever a vertex is moved.</para>
    /// <para>EdgeRouteChanged event is fired when the edge is first added and whenever any of the end-vertices are moved.</para>
    /// </remarks>
    internal sealed class IncrementalLayoutEngine
    {
        private readonly double _horizontalGap;
        private readonly double _verticalGap;
        private readonly LayoutGraph _layoutGraph;

        public List<RectMoveEventArgs> LastEdgeTriggeredVertexMoves { get; }
        public int TotalVertexMoveCount { get; private set; }

        public event EventHandler<RectMoveEventArgs> VertexCenterChanged;
        public event EventHandler<Route> EdgeRouteChanged;

        public IncrementalLayoutEngine(double horizontalGap, double verticalGap)
        {
            _horizontalGap = horizontalGap;
            _verticalGap = verticalGap;
            _layoutGraph = new LayoutGraph(horizontalGap, verticalGap);

            LastEdgeTriggeredVertexMoves = new List<RectMoveEventArgs>();
        }

        public void Clear()
        {
            _layoutGraph.Clear();
            TotalVertexMoveCount = 0;
        }

        public void Add(DiagramNode diagramNode)
        {
            Debug.WriteLine("-----------------------");
            Debug.WriteLine($"Adding node {diagramNode}");

            LastEdgeTriggeredVertexMoves.Clear();

            var layoutVertex = _layoutGraph.CreateVertex(diagramNode);
            layoutVertex.CenterChanged += OnVertexCenterChanged;

            var centerX = GetRootVertexInsertionCenterX(layoutVertex);
            MoveVertexTo(layoutVertex, centerX, new PushyOverlapResolver(_layoutGraph, _horizontalGap, centerX));
        }

        internal void Add(DiagramConnector diagramConnector)
        {
            Debug.WriteLine("-----------------------");
            Debug.WriteLine($"Adding edge {diagramConnector}");

            LastEdgeTriggeredVertexMoves.Clear();

            // TODO: ha kell, fordítsuk meg az edge-et!

            var layoutEdge = _layoutGraph.CreateEdge(diagramConnector);

            MoveTreeUnderParent(layoutEdge.Source, layoutEdge.Target);

            OnEdgeRouteChanged(layoutEdge);
        }

        private double GetRootVertexInsertionCenterX(LayoutVertex childVertex)
        {
            var previousVertex = childVertex.GetPreviousInLayer();
            var nextVertex = childVertex.GetNextInLayer();

            if (previousVertex == null && nextVertex == null)
                return 0;

            return CalculateInsertionCenterXFromNeighbours(previousVertex, nextVertex);
        }

        private double GetNonRootVertexInsertionCenterX(LayoutVertex childVertex, LayoutVertex parentVertex)
        {
            if (!childVertex.HasSiblings())
                return parentVertex.Center.X;

            var previousSibling = childVertex.GetPreviousSibling();
            var nextSibling = childVertex.GetNextSibling();

            return CalculateInsertionCenterXFromNeighbours(previousSibling, nextSibling);
        }

        private double CalculateInsertionCenterXFromNeighbours(LayoutVertex previousSibling, LayoutVertex nextSibling)
        {
            if (previousSibling == null && nextSibling == null)
                throw new ArgumentNullException(nameof(nextSibling) + " and " + nameof(previousSibling));

            return previousSibling == null
                ? nextSibling.Left - _horizontalGap / 2
                : previousSibling.Right + _horizontalGap / 2;
        }

        private void MoveTreeUnderParent(LayoutVertex childVertex, LayoutVertex parentVertex)
        {
            Debug.WriteLine($"Moving tree {childVertex} under {parentVertex}");

            FloatTree(childVertex);
            PlaceTreeUnderParent(childVertex, parentVertex);
        }

        private void FloatTree(LayoutVertex rootVertex)
        {
            Debug.WriteLine($"Floating tree {rootVertex}");

            _layoutGraph.ExecuteOnTree(rootVertex, EdgeDirection.In, i => i.IsFloating = true);
        }

        private void PlaceTreeUnderParent(LayoutVertex childVertex, LayoutVertex parentVertex)
        {
            var insertionCenterX = GetNonRootVertexInsertionCenterX(childVertex, parentVertex);

            var translateVectorX = insertionCenterX - childVertex.Center.X;
            var overlapResolver = new PushyOverlapResolver(_layoutGraph, _horizontalGap, insertionCenterX);

            _layoutGraph.ExecuteOnTree(childVertex, EdgeDirection.In, i => MoveVertexBy(i, translateVectorX, overlapResolver));
        }

        private void OnVertexCenterChanged(object sender, RectMoveEventArgs args)
        {
            PropagateVertexCenterChangedEvent((LayoutVertex)sender, args);
        }

        private void PropagateVertexCenterChangedEvent(LayoutVertex layoutVertex, RectMoveEventArgs args)
        {
            if (!layoutVertex.IsDummy)
                VertexCenterChanged?.Invoke(layoutVertex.DiagramNode, args);

            foreach (var layoutEdge in _layoutGraph.GetAllEdges(layoutVertex))
                OnEdgeRouteChanged(layoutEdge);
        }

        private void OnEdgeRouteChanged(LayoutEdge layoutEdge)
        {
            var edgeRoute = _layoutGraph.GetEdgeRoute(layoutEdge);
            EdgeRouteChanged?.Invoke(layoutEdge.DiagramConnector, edgeRoute);
        }

        private void MoveVertexTo(LayoutVertex movingVertex, double centerX, IOverlapResolver overlapResolver)
        {
            Debug.WriteLine($"Moving vertex {movingVertex} to: {movingVertex.Center.X}");

            var oldCenter = movingVertex.Center;
            var centerY = movingVertex.GetLayer().CenterY;
            movingVertex.Center = new Point2D(centerX, centerY);

            TotalVertexMoveCount++;
            if (!movingVertex.IsDummy)
                LastEdgeTriggeredVertexMoves.Add(new RectMoveEventArgs(movingVertex.DiagramNode, oldCenter, movingVertex.Center));

            ResolveOverlaps(movingVertex, overlapResolver);
        }

        private void ResolveOverlaps(LayoutVertex movingVertex, IOverlapResolver overlapResolver)
        {
            foreach (var placedVertex in movingVertex.GetOtherPositionedVerticesInLayer())
            {
                if (movingVertex.OverlapsWith(placedVertex, _horizontalGap))
                {
                    var resolution = overlapResolver.GetResolution(movingVertex, placedVertex);
                    if (resolution.WithChildren)
                        MoveTreeBy(resolution.Vertex, resolution.TranslateVectorX, overlapResolver);
                    else
                        MoveVertexBy(resolution.Vertex, resolution.TranslateVectorX, overlapResolver);

                    CenterParents(resolution.Vertex);
                }
            }
        }

        private void MoveTreeBy(LayoutVertex rootVertex, double translateVectorX, IOverlapResolver overlapResolver)
        {
            Debug.WriteLine($"Moving tree {rootVertex} by {translateVectorX}");

            FloatTree(rootVertex);

            _layoutGraph.ExecuteOnTree(rootVertex, EdgeDirection.In, i => MoveVertexBy(i, translateVectorX, overlapResolver));
        }

        private void MoveVertexBy(LayoutVertex movingVertex, double translateVectorX, IOverlapResolver overlapResolver)
        {
            Debug.WriteLine($"Moving vertex {movingVertex} by {translateVectorX}");

            MoveVertexTo(movingVertex, movingVertex.Center.X + translateVectorX, overlapResolver);
        }

        private void CenterParents(LayoutVertex layoutVertex)
        {
            foreach (var parentVertex in _layoutGraph.GetOutNeighbours(layoutVertex))
            {
                var targetCenterX = parentVertex.GetPositionedChildren().GetRect().Center.X;
                var overlapResolver = new PushyOverlapResolver(_layoutGraph, _horizontalGap, parentVertex.Center.X);
                MoveVertexTo(parentVertex, targetCenterX, overlapResolver);
                CenterParents(parentVertex);
            }
        }

        #region Unused

        private LayoutVertex ChooseVertexToMove(LayoutVertex vertex1, LayoutVertex vertex2)
        {
            var vertex1EdgeCount = _layoutGraph.Degree(vertex1);
            var vertex2EdgeCount = _layoutGraph.Degree(vertex2);

            if (vertex1EdgeCount < vertex2EdgeCount)
                return vertex1;
            if (vertex1EdgeCount > vertex2EdgeCount)
                return vertex2;

            var vertex1LayerIndex = vertex1.GetLayerIndex();
            var vertex2LayerIndex = vertex2.GetLayerIndex();

            if (vertex1LayerIndex > vertex2LayerIndex)
                return vertex1;
            if (vertex1LayerIndex < vertex2LayerIndex)
                return vertex2;

            return vertex1.Center.X > vertex2.Center.X
                ? vertex1
                : vertex2;
        }

        #endregion

    }
}
