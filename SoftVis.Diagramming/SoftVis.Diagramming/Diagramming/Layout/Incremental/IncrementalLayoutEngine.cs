using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Codartis.SoftVis.Common;
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
    internal sealed class IncrementalLayoutEngine
    {
        private readonly double _horizontalGap;
        private readonly double _verticalGap;
        private readonly LayoutGraph _layoutGraph;

        public List<RectMove> LastEdgeTriggeredVertexMoves { get; }
        public int TotalVertexMoveCount { get; private set; }

        public event EventHandler<RectMove> DiagramNodeCenterChanged;
        public event EventHandler<Route> DiagramConnectorRouteChanged;

        public IncrementalLayoutEngine(double horizontalGap, double verticalGap)
        {
            _horizontalGap = horizontalGap;
            _verticalGap = verticalGap;

            _layoutGraph = new LayoutGraph(horizontalGap, verticalGap);
            _layoutGraph.LayoutVertexCenterChanged += OnVertexCenterChanged;

            LastEdgeTriggeredVertexMoves = new List<RectMove>();
        }

        public void Clear()
        {
            _layoutGraph.Clear();

            LastEdgeTriggeredVertexMoves.Clear();
            TotalVertexMoveCount = 0;
        }

        public void Add(DiagramNode diagramNode)
        {
            Debug.WriteLine("-----------------------");

            LastEdgeTriggeredVertexMoves.Clear();

            var layoutVertex = _layoutGraph.AddNode(diagramNode);

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

            var layoutPath = _layoutGraph.AddConnector(diagramConnector);
            var lastEdgeOfPath = layoutPath.Last();

            MoveTreeUnderParent(lastEdgeOfPath.Source, lastEdgeOfPath.Target, $"Moving tree of {lastEdgeOfPath.Source} under {lastEdgeOfPath.Target}");
            Compact();
            AdjustVerticalPositions();

            FireDiagramConnectorRouteChanged(diagramConnector);
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
            if (!childVertex.HasPrimarySiblingsInSameLayer())
                return parentVertex.Center.X;

            var previousSibling = childVertex.GetPreviousSiblingInLayer();
            var nextSibling = childVertex.GetNextSiblingInLayer();

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
            childVertex.FloatPrimaryTree();
            PlaceTreeUnderParent(childVertex, parentVertex, reason);
        }

        private void PlaceTreeUnderParent(LayoutVertex childVertex, LayoutVertex parentVertex, string reason)
        {
            var insertionCenterX = GetNonRootVertexInsertionCenterX(childVertex, parentVertex);
            var translateVectorX = insertionCenterX - childVertex.Center.X;

            childVertex.ExecuteOnPrimaryTree(i => MoveVertexBy(i, translateVectorX, reason));
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

                if (!movingVertex.IsDummy)
                {
                    TotalVertexMoveCount++;
                    LastEdgeTriggeredVertexMoves.Add(new RectMove(movingVertex.DiagramNode, oldCenter, movingVertex.Center));
                }
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
            rootVertex.FloatPrimaryTree();
            rootVertex.ExecuteOnPrimaryTree(i => MoveVertexBy(i, translateVectorX, $"{reason} --> Moving tree {rootVertex} by {translateVectorX}"));
        }

        private void MoveVertexBy(LayoutVertex movingVertex, double translateVectorX, string reason)
        {
            MoveVertexTo(movingVertex, movingVertex.Center.X + translateVectorX, reason);
        }

        private void CenterParents(LayoutVertex layoutVertex)
        {
            var parentVertex = layoutVertex.GetPrimaryParent();
            if (parentVertex == null)
                return;

            var targetCenterX = parentVertex.GetPositionedChildren().GetRect().Center.X;
            MoveVertexTo(parentVertex, targetCenterX, $"Centering parents of {layoutVertex}");
            CenterParents(parentVertex);

            // TODO: how to place non-primary parents?
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

                    if (leftVertex.GetPrimaryParent() != rightVertex.GetPrimaryParent())
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

        private void OnVertexCenterChanged(object sender, RectMove args)
        {
            var layoutVertex = (LayoutVertex) sender;

            if (!layoutVertex.IsDummy)
                DiagramNodeCenterChanged?.Invoke(layoutVertex.DiagramNode, args);

            foreach (var layoutEdge in _layoutGraph.GetAllEdges(layoutVertex))
                FireDiagramConnectorRouteChanged(layoutEdge.DiagramConnector);
        }

        private void FireDiagramConnectorRouteChanged(DiagramConnector diagramConnector)
        {
            var route = _layoutGraph.GetRouteForDiagramConnector(diagramConnector);
            DiagramConnectorRouteChanged?.Invoke(diagramConnector, route);
        }
    }
}
