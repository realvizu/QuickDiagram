using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Codartis.SoftVis.Common;
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

            LastEdgeTriggeredVertexMoves.Clear();

            var layoutVertex = _layoutGraph.CreateVertex(diagramNode);
            layoutVertex.CenterChanged += OnVertexCenterChanged;

            var centerX = GetRootVertexInsertionCenterX(layoutVertex);
            MoveVertexTo(layoutVertex, centerX, $"Adding node {diagramNode}");
            Compact();
            AdjustVerticalPositions();
        }

        public void Add(DiagramConnector diagramConnector)
        {
            Debug.WriteLine("-----------------------");
            Debug.WriteLine($"Adding edge {diagramConnector}, moving {diagramConnector.Source}");

            LastEdgeTriggeredVertexMoves.Clear();

            // TODO: turn edge direction if needed!

            var layoutEdge = _layoutGraph.CreateEdge(diagramConnector);

            MoveTreeUnderParent(layoutEdge.Source, layoutEdge.Target, $"Moving tree {layoutEdge.Source} under {layoutEdge.Target}");
            Compact();
            AdjustVerticalPositions();

            OnEdgeRouteChanged(layoutEdge);
        }

        private double GetRootVertexInsertionCenterX(LayoutVertex rootVertex)
        {
            var previousVertex = rootVertex.GetPreviousInLayer();
            var nextVertex = rootVertex.GetNextInLayer();

            return (previousVertex == null && nextVertex == null)
                ? 0
                : CalculateInsertionCenterXFromNeighbours(previousVertex, nextVertex);
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

        private void MoveTreeUnderParent(LayoutVertex childVertex, LayoutVertex parentVertex, string reason)
        {
            childVertex.FloatTree();
            PlaceTreeUnderParent(childVertex, parentVertex, reason);
        }

        private void PlaceTreeUnderParent(LayoutVertex childVertex, LayoutVertex parentVertex, string reason)
        {
            var insertionCenterX = GetNonRootVertexInsertionCenterX(childVertex, parentVertex);
            var translateVectorX = insertionCenterX - childVertex.Center.X;

            childVertex.ExecuteOnTree(i => MoveVertexBy(i, translateVectorX, reason));
        }

        private void MoveVertexTo(LayoutVertex movingVertex, double centerX, string reason)
        {
            movingVertex.IsFloating = false;

            var centerY = movingVertex.GetLayer().CenterY;

            if (!movingVertex.Center.X.IsEqualWithTolerance(centerX) ||
                !movingVertex.Center.Y.IsEqualWithTolerance(centerY))
            {
                Debug.WriteLine($"{reason} --> Moving vertex {movingVertex} to: {centerX}");

                var oldCenter = movingVertex.Center;

                movingVertex.Center = new Point2D(centerX, centerY);

                TotalVertexMoveCount++;
                if (!movingVertex.IsDummy)
                    LastEdgeTriggeredVertexMoves.Add(new RectMoveEventArgs(movingVertex.DiagramNode, oldCenter, movingVertex.Center));
            }

            PushOtherVerticesFromTheWay(movingVertex);
        }

        private void PushOtherVerticesFromTheWay(LayoutVertex pushyVertex)
        {
            var pushyVertexIndex = pushyVertex.GetIndexInLayer();
            foreach (var existingVertex in pushyVertex.GetOtherPositionedVerticesInLayer())
            {
                var existingVertexIndex = existingVertex.GetIndexInLayer();

                if (existingVertexIndex < pushyVertexIndex)
                    EnsureVertexIsToTheLeft(existingVertex, pushyVertex);
                else if (existingVertexIndex > pushyVertexIndex)
                    EnsureVertexIsToTheRight(existingVertex, pushyVertex);
            }
        }

        private void EnsureVertexIsToTheRight(LayoutVertex rightVertex, LayoutVertex leftVertex)
        {
            var vertexDistance = rightVertex.Left - leftVertex.Right;
            if (vertexDistance >= _horizontalGap)
                return;

            MoveTreeBy(rightVertex, _horizontalGap - vertexDistance, $"Vertex {leftVertex} is pushing vertex {rightVertex} to the right");
            CenterParents(rightVertex);
        }

        private void EnsureVertexIsToTheLeft(LayoutVertex leftVertex, LayoutVertex rightVertex)
        {
            var vertexDistance = rightVertex.Left - leftVertex.Right;
            if (vertexDistance >= _horizontalGap)
                return;

            MoveTreeBy(leftVertex, vertexDistance - _horizontalGap, $"Vertex {rightVertex} is pushing vertex {leftVertex} to the left");
            CenterParents(leftVertex);
        }

        private void MoveTreeBy(LayoutVertex rootVertex, double translateVectorX, string reason)
        {
            rootVertex.FloatTree();
            rootVertex.ExecuteOnTree(i => MoveVertexBy(i, translateVectorX, $"{reason} --> Moving tree {rootVertex} by {translateVectorX}"));
        }

        private void MoveVertexBy(LayoutVertex movingVertex, double translateVectorX, string reason)
        {
            MoveVertexTo(movingVertex, movingVertex.Center.X + translateVectorX, reason);
        }

        private void CenterParents(LayoutVertex layoutVertex)
        {
            foreach (var parentVertex in layoutVertex.GetParents())
            {
                var targetCenterX = parentVertex.GetPositionedChildren().GetRect().Center.X;
                MoveVertexTo(parentVertex, targetCenterX, $"Centering parents of {layoutVertex}");
                CenterParents(parentVertex);
            }
        }

        private void Compact()
        {
            CompactVertically();
            CompactHorizontally();
        }

        private void CompactVertically()
        {
            //TODO: remove empty and dummy-only layers
        }

        private void CompactHorizontally()
        {
            foreach (var layer in _layoutGraph.Layers)
            {
                for (var i = 1; i < layer.Count; i++)
                {
                    var leftVertex = layer[i - 1];
                    var rightVertex = layer[i];

                    if (leftVertex.GetParent() != rightVertex.GetParent())
                        continue;

                    var gap = DetermineGap(leftVertex, rightVertex);
                    if (gap > _horizontalGap)
                    {
                        var translate = -gap + _horizontalGap;
                        MoveTreeBy(rightVertex, translate, $"Compacting {rightVertex} (layer {layer.LayerIndex}) by {translate}");
                        CenterParents(rightVertex);
                    }
                }
            }
        }

        private double DetermineGap(LayoutVertex leftVertex, LayoutVertex rightVertex)
        {
            var gap = rightVertex.Left - leftVertex.Right;
            if (gap <= _horizontalGap)
                return gap;
            
            var rightVertexLeftmostChild = rightVertex.GetChildren().OrderBy(i => i.Right).FirstOrDefault();
            if (rightVertexLeftmostChild == null)
                return gap;
            
            var previousVertex = rightVertexLeftmostChild.GetPreviousInLayer();
            if (previousVertex == null)
                return gap;
            
            var childrenGap = DetermineGap(previousVertex, rightVertexLeftmostChild);
            var result = Math.Min(childrenGap, gap);
            return result;
        }

        private void AdjustVerticalPositions()
        {
            foreach (var layoutVertex in _layoutGraph.Vertices)
                layoutVertex.RefreshVerticalPosition();
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
