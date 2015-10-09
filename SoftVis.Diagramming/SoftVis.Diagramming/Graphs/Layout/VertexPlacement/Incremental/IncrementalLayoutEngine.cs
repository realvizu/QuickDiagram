using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Codartis.SoftVis.Common;
using Codartis.SoftVis.Geometry;
using QuickGraph;

namespace Codartis.SoftVis.Graphs.Layout.VertexPlacement.Incremental
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
        private readonly Layers _layers;
        private readonly Dictionary<IRect, LayoutVertex> _originalToLayoutVertexMap;

        public List<RectMove> LastEdgeTriggeredVertexMoves { get; }
        public int TotalVertexMoveCount { get; private set; }

        public event EventHandler<MoveEventArgs> VertexCenterChanged;
        public event EventHandler<Route> EdgeRouteChanged;

        public IncrementalLayoutEngine(double horizontalGap, double verticalGap)
        {
            _horizontalGap = horizontalGap;
            _verticalGap = verticalGap;

            _layoutGraph = new LayoutGraph();
            _layers = new Layers(_layoutGraph, _verticalGap);
            _originalToLayoutVertexMap = new Dictionary<IRect, LayoutVertex>();
            LastEdgeTriggeredVertexMoves = new List<RectMove>();
        }

        public void Clear()
        {
            _layoutGraph.Clear();
            _originalToLayoutVertexMap.Clear();
            TotalVertexMoveCount = 0;
        }

        public void Add(IRect originalVertex)
        {
            Debug.WriteLine($"Adding node {originalVertex}");

            LastEdgeTriggeredVertexMoves.Clear();

            var newLayoutVertex = CreateLayoutVertex(originalVertex);

            _layoutGraph.AddVertex(newLayoutVertex);
            _originalToLayoutVertexMap.Add(originalVertex, newLayoutVertex);

            var alignToRect = _layers.First().Rect.WithMarginX(_horizontalGap);
            var centerX = originalVertex.Rect.OuterAlignedTo(alignToRect, Side.Right, VerticalAlignment.Center).Center.X;
            MoveVertexTo(newLayoutVertex, centerX, null);
        }

        internal void Add(IEdge<IRect> originalEdge)
        {
            Debug.WriteLine($"Adding edge {originalEdge}");

            LastEdgeTriggeredVertexMoves.Clear();

            // TODO: ha kell, fordítsuk meg az edge-et!

            var newLayoutEdge = CreateLayoutEdge(originalEdge);

            _layoutGraph.AddEdge(newLayoutEdge);
            MoveTreeUnderParent(newLayoutEdge.Source, newLayoutEdge.Target);

            OnEdgeRouteChanged(newLayoutEdge);
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
            var nonFloatingSiblings = _layoutGraph.GetNonFloatingNeighbours(parentVertex, EdgeDirection.In).ToArray();
            EnsureSiblingsAreSameRank(childVertex, nonFloatingSiblings);

            var insertionCenterX = nonFloatingSiblings.Any()
                ? nonFloatingSiblings.GetInsertionPointX(childVertex, _horizontalGap)
                : parentVertex.Center.X;

            var translateVectorX = insertionCenterX - childVertex.Center.X;
            var overlapResolver = new PushyOverlapResolver(_layoutGraph, _horizontalGap, insertionCenterX);

            _layoutGraph.ExecuteOnTree(childVertex, EdgeDirection.In, i => MoveVertexBy(i, translateVectorX, overlapResolver));
        }

        private void EnsureSiblingsAreSameRank(LayoutVertex childVertex, IEnumerable<LayoutVertex> siblings)
        {
            // TODO: a siblingek ne lehessenek különböző layerekben -> dummy node-ok bevezetése
            var childLayerIndex = _layers.GetLayerIndex(childVertex);
            if (siblings.Any(i => _layers.GetLayerIndex(i) != childLayerIndex))
                throw new Exception("Dummy vertexek kellenek!");
        }

        private void OnVertexCenterChanged(object sender, MoveEventArgs args)
        {
            PropagateVertexCenterChangedEvent((LayoutVertex)sender, args);
        }

        private void PropagateVertexCenterChangedEvent(LayoutVertex layoutVertex, MoveEventArgs args)
        {
            if (!layoutVertex.IsDummy)
                VertexCenterChanged?.Invoke(layoutVertex.OriginalVertex, args);

            foreach (var layoutEdge in _layoutGraph.GetAllEdges(layoutVertex))
                OnEdgeRouteChanged(layoutEdge);
        }

        private void OnEdgeRouteChanged(LayoutEdge layoutEdge)
        {
            var edgeRoute = _layoutGraph.GetEdgeRoute(layoutEdge);
            EdgeRouteChanged?.Invoke(layoutEdge.OriginalEdge, edgeRoute);
        }

        private LayoutVertex CreateLayoutVertex(IRect originalVertex)
        {
            var newLayoutVertex = LayoutVertex.Create(originalVertex, isFloating: true);
            newLayoutVertex.CenterChanged += OnVertexCenterChanged;
            return newLayoutVertex;
        }

        private LayoutEdge CreateLayoutEdge(IEdge<IRect> originalEdge)
        {
            var sourceLayoutVertex = _originalToLayoutVertexMap[originalEdge.Source];
            var targetLayoutVertex = _originalToLayoutVertexMap[originalEdge.Target];
            return new LayoutEdge(originalEdge, sourceLayoutVertex, targetLayoutVertex);
        }

        private void MoveVertexTo(LayoutVertex movingVertex, double centerX, IOverlapResolver overlapResolver)
        {
            var centerY = _layers.GetCenterY(movingVertex);

            if (movingVertex.Center.X.IsEqualWithTolerance(centerX) &&
                movingVertex.Center.Y.IsEqualWithTolerance(centerY))
                return;

            Debug.WriteLine($"Moving vertex {movingVertex} to: {movingVertex.Center.X}");

            var oldCenter = movingVertex.Center;
            movingVertex.Center = new Point2D(centerX, centerY);

            TotalVertexMoveCount++;
            if (!movingVertex.IsDummy)
                LastEdgeTriggeredVertexMoves.Add(new RectMove(movingVertex.OriginalVertex, oldCenter, movingVertex.Center));

            ResolveOverlaps(movingVertex, overlapResolver);
        }

        private void ResolveOverlaps(LayoutVertex movingVertex, IOverlapResolver overlapResolver)
        {
            foreach (var placedVertex in _layers.GetOtherNonFloatingVerticesInLayer(movingVertex))
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
                var targetCenterX = _layoutGraph.GetNonFloatingNeighbours(parentVertex, EdgeDirection.In).GetRect().Center.X;
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

            var vertex1LayerIndex = _layers.GetLayerIndex(vertex1);
            var vertex2LayerIndex = _layers.GetLayerIndex(vertex2);

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
