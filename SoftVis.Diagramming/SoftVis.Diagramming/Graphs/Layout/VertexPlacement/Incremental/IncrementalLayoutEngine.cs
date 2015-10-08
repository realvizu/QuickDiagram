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
    /// Sends events about vertex position changes.
    /// </summary>
    internal sealed class IncrementalLayoutEngine
    {
        private readonly double _horizontalGap;
        private readonly double _verticalGap;
        private readonly LayoutGraph _layoutGraph;
        private readonly Layers _layers;
        private readonly Dictionary<IRect, LayoutVertex> _originalToLayoutVertexMap;
        private readonly IDictionary<IEdge<IRect>, IList<LayoutVertex>> _edgeToDummyVerticesMap;

        public event EventHandler<MoveEventArgs> VertexCenterChanged;
        public event EventHandler<Route> EdgeRouteChanged;

        public IncrementalLayoutEngine(double horizontalGap, double verticalGap)
        {
            _horizontalGap = horizontalGap;
            _verticalGap = verticalGap;

            _layoutGraph = new LayoutGraph();
            _layers = new Layers(_layoutGraph, verticalGap);
            _originalToLayoutVertexMap = new Dictionary<IRect, LayoutVertex>();
            _edgeToDummyVerticesMap = new Dictionary<IEdge<IRect>, IList<LayoutVertex>>();
        }

        public void Add(IRect originalVertex)
        {
            var newLayoutVertex = CreateLayoutVertex(originalVertex);

            _layoutGraph.AddVertex(newLayoutVertex);
            _originalToLayoutVertexMap.Add(originalVertex, newLayoutVertex);

            var alignToRect = _layers.First().Rect.WithMarginX(_horizontalGap);
            newLayoutVertex.Center = originalVertex.Rect.OuterAlignedTo(alignToRect, Side.Right, VerticalAlignment.Center).Center;
        }

        internal void Add(IEdge<IRect> originalEdge)
        {
            // TODO: ha kell, fordítsuk meg az edge-et!

            var newLayoutEdge = CreateLayoutEdge(originalEdge);

            _layoutGraph.AddEdge(newLayoutEdge);
            MoveTreeUnderParent(newLayoutEdge.Source, newLayoutEdge.Target);

            OnEdgeRouteChanged(newLayoutEdge);
        }

        private void MoveTreeUnderParent(LayoutVertex childVertex, LayoutVertex parentVertex)
        {
            FloatTree(childVertex);
            PlaceTreeUnderParent(childVertex, parentVertex);
        }

        private void FloatTree(LayoutVertex rootVertex)
        {
            _layoutGraph.ExecuteOnTree(rootVertex, EdgeDirection.In, i => i.IsFloating = true);
        }

        private void PlaceTreeUnderParent(LayoutVertex childVertex, LayoutVertex parentVertex)
        {
            var translateVectorX = CalculateVectorToMoveUnderParent(childVertex, parentVertex);
            _layoutGraph.ExecuteOnTree(childVertex, EdgeDirection.In,
                i => MoveVertexBy(i, translateVectorX, new PushyOverlapResolver(_horizontalGap)));
        }

        private double CalculateVectorToMoveUnderParent(LayoutVertex childVertex, LayoutVertex parentVertex)
        {
            var siblings = _layoutGraph.GetNonFloatingNeighbours(parentVertex, EdgeDirection.In).ToArray();
            EnsureSiblingsAreSameRank(childVertex, siblings);
            var insertionCenterX = siblings.GetInsertionPointX(childVertex, parentVertex, _horizontalGap);
            return insertionCenterX - childVertex.Center.X;
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
            var edgeRoute = GetEdgeRoute(layoutEdge);
            EdgeRouteChanged?.Invoke(layoutEdge.OriginalEdge, edgeRoute);
        }

        private Route GetEdgeRoute(LayoutEdge layoutEdge)
        {
            var sourceRect = layoutEdge.Source.Rect;
            var targetRect = layoutEdge.Target.Rect;

            return new Route
            {
                sourceRect.GetAttachPointToward(targetRect.Center),
                GetInterimRoutePoints(layoutEdge),
                targetRect.GetAttachPointToward(sourceRect.Center)
            };
        }

        private IEnumerable<Point2D> GetInterimRoutePoints(LayoutEdge layoutEdge)
        {
            IList<LayoutVertex> dummyVertices;
            return _edgeToDummyVerticesMap.TryGetValue(layoutEdge.OriginalEdge, out dummyVertices)
                ? dummyVertices.Select(i => i.Center)
                : null;
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

        private void CenterParents(LayoutVertex layoutVertex)
        {
            foreach (var parentVertex in _layoutGraph.GetOutNeighbours(layoutVertex))
            {
                var targetCenterX = _layoutGraph.GetNonFloatingNeighbours(parentVertex, EdgeDirection.In).GetRect().Center.X;
                var overlapResolver = new BackSlidingOverlapResolver(_horizontalGap, parentVertex.Center.X);
                MoveVertexTo(parentVertex, targetCenterX, overlapResolver);
            }
        }

        private void MoveTreeBy(LayoutVertex rootVertex, double translateVectorX, IOverlapResolver overlapResolver)
        {
            _layoutGraph.ExecuteOnTree(rootVertex, EdgeDirection.In, i => MoveVertexBy(i, translateVectorX, overlapResolver));
        }

        private void MoveVertexBy(LayoutVertex movingVertex, double translateVectorX, IOverlapResolver overlapResolver)
        {
            MoveVertexTo(movingVertex, movingVertex.Center.X + translateVectorX, overlapResolver);
        }

        private void MoveVertexTo(LayoutVertex movingVertex, double centerX, IOverlapResolver overlapResolver)
        {
            if (movingVertex.Center.X.IsEqualWithTolerance(centerX))
                return;

            Debug.WriteLine($"Vertex {movingVertex} moving to X: {movingVertex.Center.X}");

            movingVertex.Center = new Point2D(centerX, _layers.GetCenterY(movingVertex));
            ResolveOverlaps(movingVertex, overlapResolver);
            CenterParents(movingVertex);
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
                }
            }
        }

        #region Unused

        public void BreakEdgeWithInterimVertices(LayoutEdge edgeToBreak, List<LayoutVertex> interimVertices)
        {
            SaveEdgeToDummyVerticesMapping(edgeToBreak, interimVertices);

            _layoutGraph.RemoveEdge(edgeToBreak);

            _layoutGraph.AddVertexRange(interimVertices);

            var vertexPath = edgeToBreak.Source.ToEnumerable().Concat(interimVertices).Concat(edgeToBreak.Target).ToArray();

            for (var i = 0; i < vertexPath.Length - 1; i++)
            {
                var newEdge = new LayoutEdge(edgeToBreak.OriginalEdge, vertexPath[i], vertexPath[i + 1], edgeToBreak.IsReversed);
                _layoutGraph.AddEdge(newEdge);
            }
        }

        private void SaveEdgeToDummyVerticesMapping(LayoutEdge edge, IEnumerable<LayoutVertex> interimVertices)
        {
            interimVertices = edge.IsReversed ? interimVertices.Reverse() : interimVertices;

            _edgeToDummyVerticesMap.Add(edge.OriginalEdge, interimVertices.ToArray());
        }

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
