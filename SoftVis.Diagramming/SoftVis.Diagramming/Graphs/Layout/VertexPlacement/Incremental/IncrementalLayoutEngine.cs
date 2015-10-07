using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Media.Animation;
using Codartis.SoftVis.Common;
using Codartis.SoftVis.Geometry;
using QuickGraph;

namespace Codartis.SoftVis.Graphs.Layout.VertexPlacement.Incremental
{
    /// <summary>
    /// Calculates positions whenever vertices and edges are added.
    /// Sends events about vertex position changes.
    /// </summary>
    /// <remarks>
    /// When a node moves it takes its children with it.
    /// </remarks>
    internal sealed class IncrementalLayoutEngine
    {
        private readonly double _horizontalGap;
        private readonly double _verticalGap;
        private readonly LayoutGraph _layoutGraph;
        private readonly RankLayers _rankLayers;
        private readonly Dictionary<IRect, LayoutVertex> _originalToLayoutVertexMap;
        private readonly IDictionary<IEdge<IRect>, IList<LayoutVertex>> _edgeToDummyVerticesMap;

        public event EventHandler<MoveEventArgs> VertexCenterChanged;
        public event EventHandler<Route> EdgeRouteChanged;

        public IncrementalLayoutEngine(double horizontalGap, double verticalGap)
        {
            _horizontalGap = horizontalGap;
            _verticalGap = verticalGap;

            _layoutGraph = new LayoutGraph();
            _rankLayers = new RankLayers(horizontalGap, verticalGap, _layoutGraph);
            _originalToLayoutVertexMap = new Dictionary<IRect, LayoutVertex>();
            _edgeToDummyVerticesMap = new Dictionary<IEdge<IRect>, IList<LayoutVertex>>();
        }

        public void Add(IRect originalVertex)
        {
            var newLayoutVertex = CreateLayoutVertex(originalVertex);
            newLayoutVertex.CenterChanged += OnVertexCenterChanged;

            _originalToLayoutVertexMap.Add(originalVertex, newLayoutVertex);

            _layoutGraph.AddVertex(newLayoutVertex);
            _rankLayers.AddVertex(0, newLayoutVertex);
        }

        internal void Add(IEdge<IRect> originalEdge)
        {
            // TODO: nem kell megfordítani az edge-et??

            var newLayoutEdge = CreateLayoutEdge(originalEdge);

            _layoutGraph.AddEdge(newLayoutEdge);
            MoveTreeUnderParent(newLayoutEdge.Source, newLayoutEdge.Target);

            OnEdgeRouteChanged(newLayoutEdge);

            //foreach (var root in _rankLayers[0])
            //    DumpCenters(root);
        }

        private void MoveTreeUnderParent(LayoutVertex childVertex, LayoutVertex parentVertex)
        {
            RemoveTreeFromRankLayers(childVertex);
            AddTreeToRankLayers(childVertex, parentVertex);

            //MoveTreeUnderParentInRankLayers(childVertex, parentVertex);
            //MoveTreeUnderParentInLayout(childVertex, parentVertex);
        }

        private void AddTreeToRankLayers(LayoutVertex childVertex, LayoutVertex parentVertex)
        {
            _rankLayers.AddVertex(parentVertex.Rank.Value + 1, childVertex);
            MoveChildUnderParent(childVertex, parentVertex);

            _layoutGraph.ExecuteOnTreeEdges(childVertex, EdgeDirection.In,
                i =>
                {
                    _rankLayers.AddVertex(i.Target.Rank.Value + 1, i.Source);
                    MoveChildUnderParent(i.Source, i.Target);
                });
        }

        private void RemoveTreeFromRankLayers(LayoutVertex rootVertex)
        {
            _rankLayers.RemoveVertex(rootVertex);
            _layoutGraph.ExecuteOnTreeEdges(rootVertex, EdgeDirection.In,
                i => _rankLayers.RemoveVertex(i.Source));
        }

        private void MoveTreeUnderParentInRankLayers(LayoutVertex childVertex, LayoutVertex parentVertex)
        {
            _rankLayers.EnsureVertexIsUnderParentVertex(childVertex, parentVertex);
            _layoutGraph.ExecuteOnTreeEdges(childVertex, EdgeDirection.In,
                i => _rankLayers.EnsureVertexIsUnderParentVertex(i.Source, i.Target));
        }

        private void MoveTreeUnderParentInLayout(LayoutVertex childVertex, LayoutVertex parentVertex)
        {
            MoveChildUnderParent(childVertex, parentVertex);
            _layoutGraph.ExecuteOnTreeEdges(childVertex, EdgeDirection.In, i => MoveChildUnderParent(i.Source, i.Target));
        }

        private void MoveChildUnderParent(LayoutVertex childVertex, LayoutVertex parentVertex)
        {
            var siblings = _layoutGraph.GetSiblings(childVertex, parentVertex, EdgeDirection.In).Where(i => i.Rank != null).ToArray();
            EnsureSiblingsAreSameRank(childVertex, siblings);
            var insertionCenterX = siblings.GetInsertionPointX(childVertex, parentVertex, _horizontalGap);

            _rankLayers.MoveVertexCenterXTo(childVertex, insertionCenterX);
        }

        private static void EnsureSiblingsAreSameRank(LayoutVertex childVertex, IEnumerable<LayoutVertex> siblings)
        {
            // TODO: a siblingek ne lehessenek különböző layerekben -> dummy node-ok bevezetése
            if (siblings.Any(i => i.Rank != childVertex.Rank))
                throw new Exception("Dummy vertexek kellenek!");
        }

        private void OnVertexCenterChanged(object sender, MoveEventArgs args)
        {
            var layoutVertex = (LayoutVertex)sender;
            PropagateVertexCenterChangedEvent(layoutVertex, args);

            MoveChildrenWithParent(layoutVertex, args.From, args.To);
            //BalanceParents(layoutVertex);
        }

        private void BalanceParents(LayoutVertex layoutVertex)
        {
            var parents = _layoutGraph.GetOutNeighbours(layoutVertex);
            foreach (var parentVertex in parents)
            {
                var targetCenterX = _layoutGraph.GetInNeighbours(parentVertex).Where(i => i.Rank != null).Select(i => i.Rect).Union().Center.X;
                _rankLayers.MoveVertexCenterXBy(parentVertex, targetCenterX - parentVertex.Center.X);
            }
        }

        private void MoveChildrenWithParent(LayoutVertex movedVertex, Point2D @from, Point2D to)
        {
            var translateVectorX = to.X - @from.X;
            _layoutGraph.GetInNeighbours(movedVertex).ForEach(i =>
            {
                if (i.Rank != null)
                {
                    Debug.WriteLine($"Moving vertex {i} with parent {movedVertex} by {translateVectorX}.");
                    _rankLayers.MoveVertexCenterXBy(i, translateVectorX);
                }
            });
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

        private static LayoutVertex CreateLayoutVertex(IRect originalVertex)
        {
            return LayoutVertex.Create(originalVertex);
        }

        private LayoutEdge CreateLayoutEdge(IEdge<IRect> originalEdge)
        {
            var sourceLayoutVertex = _originalToLayoutVertexMap[originalEdge.Source];
            var targetLayoutVertex = _originalToLayoutVertexMap[originalEdge.Target];
            return new LayoutEdge(originalEdge, sourceLayoutVertex, targetLayoutVertex);
        }

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
            if (vertex1.Rank == null) throw new InvalidOperationException($"{nameof(vertex1)} has no rank assigned.");
            if (vertex2.Rank == null) throw new InvalidOperationException($"{nameof(vertex2)} has no rank assigned.");

            var vertex1EdgeCount = _layoutGraph.Degree(vertex1);
            var vertex2EdgeCount = _layoutGraph.Degree(vertex2);

            if (vertex1EdgeCount < vertex2EdgeCount)
                return vertex1;
            if (vertex1EdgeCount > vertex2EdgeCount)
                return vertex2;
            if (vertex1.Rank > vertex2.Rank)
                return vertex1;
            if (vertex1.Rank < vertex2.Rank)
                return vertex2;

            return vertex1.Center.X > vertex2.Center.X
                ? vertex1
                : vertex2;
        }

        private void DumpCenters(LayoutVertex root)
        {
            var childrenCenterX = _layoutGraph.GetInNeighbours(root).GetRect().Center.X;
            Debug.WriteLine($"Root {root} center: {root.Center.X}, children center: {childrenCenterX}");

            foreach (var inNeighbour in _layoutGraph.GetInNeighbours(root))
                DumpCenters(inNeighbour);
        }
    }
}
