using System;
using System.Collections.Generic;
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
        private readonly RankLayers _rankLayers;
        private readonly Dictionary<IRect, LayoutVertex> _originalToLayoutVertexMap;
        private readonly IDictionary<IEdge<IRect>, IList<LayoutVertex>> _edgeToDummyVerticesMap;

        public event EventHandler<Point2D> VertexCenterChanged;
        public event EventHandler<Point2D[]> EdgeRouteChanged;

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
        }

        private void MoveTreeUnderParent(LayoutVertex childVertex, LayoutVertex parentVertex)
        {
            MoveChildUnderParent(childVertex, parentVertex);

            //_layoutGraph.TraverseTreeEdges(childVertex, EdgeDirection.In, i => MoveChildUnderParent(i.Source, i.Target));
        }

        private void MoveChildUnderParent(LayoutVertex childVertex, LayoutVertex parentVertex)
        {
            _rankLayers.EnsureOneVertexIsAboveTheOther(parentVertex, childVertex);

            var siblings = _layoutGraph.GetSiblings(childVertex, parentVertex, EdgeDirection.In).ToArray();

            // TODO: a siblingek ne lehessenek különböző layerekben -> dummy node-ok bevezetése
            if (siblings.Any(i => i.Rank != childVertex.Rank))
                throw new Exception("Dummy vertexek kellenek!");

            var insertionCenterX = siblings.GetInsertionPointX(childVertex, parentVertex, _horizontalGap);

            _rankLayers.MoveVertexCenterXTo(childVertex, insertionCenterX);
        }

        private void OnVertexCenterChanged(object sender, Point2D point2D)
        {
            var layoutVertex = (LayoutVertex)sender;

            if (!layoutVertex.IsDummy)
                VertexCenterChanged?.Invoke(layoutVertex.OriginalVertex, point2D);

            foreach (var layoutEdge in _layoutGraph.GetAllEdges(layoutVertex))
                OnEdgeRouteChanged(layoutEdge);
        }

        private void OnEdgeRouteChanged(LayoutEdge layoutEdge)
        {
            var edgeRoute = GetEdgeRoute(layoutEdge).ToArray();
            EdgeRouteChanged?.Invoke(layoutEdge.OriginalEdge, edgeRoute);
        }

        private IEnumerable<Point2D> GetEdgeRoute(LayoutEdge layoutEdge)
        {
            var sourceRect = layoutEdge.Source.Rect;
            var targetRect = layoutEdge.Target.Rect;

            yield return sourceRect.GetAttachPointToward(targetRect.Center);

            IList<LayoutVertex> dummyVertices;
            if (_edgeToDummyVerticesMap.TryGetValue(layoutEdge.OriginalEdge, out dummyVertices))
                foreach (var dummyVertex in dummyVertices)
                    yield return dummyVertex.Center;

            yield return targetRect.GetAttachPointToward(sourceRect.Center);
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

            return _rankLayers.GetLayer(vertex1).IndexOf(vertex1) > _rankLayers.GetLayer(vertex2).IndexOf(vertex2)
                ? vertex1
                : vertex2;
        }
    }
}
