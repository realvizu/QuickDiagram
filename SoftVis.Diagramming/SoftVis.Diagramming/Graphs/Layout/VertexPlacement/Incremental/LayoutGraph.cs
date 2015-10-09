using System;
using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Common;
using Codartis.SoftVis.Geometry;
using MoreLinq;
using QuickGraph;

namespace Codartis.SoftVis.Graphs.Layout.VertexPlacement.Incremental
{
    /// <summary>
    /// A graph used for calculating layout (vertex positions and edge routes).
    /// </summary>
    /// <remarks>
    /// Differences to the original graph:
    /// <para>Dummy vertices can be added to break long edges and ensure that neighbours are always on adjacent layers.</para>
    /// <para>Edges can be reversed to ensure an acyclic graph.</para>
    /// <para>When rearranging vertices the "floating" ones are not considered so they don't cause overlaps.</para>
    /// </remarks>
    internal sealed class LayoutGraph : BidirectionalGraph<LayoutVertex, LayoutEdge>
    {
        private readonly IDictionary<IEdge<IRect>, IList<LayoutVertex>> _edgeToDummyVerticesMap;

        public LayoutGraph()
        {
            _edgeToDummyVerticesMap = new Dictionary<IEdge<IRect>, IList<LayoutVertex>>();
            Cleared += OnCleared;
        }

        public IEnumerable<LayoutVertex> GetNonFloatingNeighbours(LayoutVertex layoutVertex, EdgeDirection edgeDirection)
        {
            return this.GetNeighbours(layoutVertex, edgeDirection).Where(i => !i.IsFloating);
        }

        public Route GetEdgeRoute(LayoutEdge layoutEdge)
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

        private void OnCleared(object sender, EventArgs args)
        {
            _edgeToDummyVerticesMap.Clear();
        }

        #region Unused

        private void BreakEdgeWithInterimVertices(LayoutEdge edgeToBreak, List<LayoutVertex> interimVertices)
        {
            SaveEdgeToDummyVerticesMapping(edgeToBreak, interimVertices);

            RemoveEdge(edgeToBreak);

            AddVertexRange(interimVertices);

            var vertexPath = edgeToBreak.Source.ToEnumerable().Concat(interimVertices).Concat(edgeToBreak.Target).ToArray();

            for (var i = 0; i < vertexPath.Length - 1; i++)
            {
                var newEdge = new LayoutEdge(edgeToBreak.OriginalEdge, vertexPath[i], vertexPath[i + 1], edgeToBreak.IsReversed);
                AddEdge(newEdge);
            }
        }

        private void SaveEdgeToDummyVerticesMapping(LayoutEdge edge, IEnumerable<LayoutVertex> interimVertices)
        {
            interimVertices = edge.IsReversed ? interimVertices.Reverse() : interimVertices;

            _edgeToDummyVerticesMap.Add(edge.OriginalEdge, interimVertices.ToArray());
        }

        #endregion
    }
}
