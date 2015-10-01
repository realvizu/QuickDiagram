using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Codartis.SoftVis.Common;
using QuickGraph;

namespace Codartis.SoftVis.Diagramming.Graph.Layout.VertexPlacement.Incremental
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
        private readonly Dictionary<IPositionedExtent, LayoutVertex> _originalToLayoutVertexMap;
        private readonly IDictionary<IEdge<IPositionedExtent>, IList<LayoutVertex>> _edgeToDummyVerticesMap;

        public event EventHandler<DiagramPoint> VertexCenterChanged;
        public event EventHandler<DiagramPoint[]> EdgeRouteChanged;

        public IncrementalLayoutEngine(double horizontalGap, double verticalGap)
        {
            _horizontalGap = horizontalGap;
            _verticalGap = verticalGap;

            _layoutGraph = new LayoutGraph();
            _rankLayers = new RankLayers(horizontalGap, verticalGap, _layoutGraph);
            _originalToLayoutVertexMap = new Dictionary<IPositionedExtent, LayoutVertex>();
            _edgeToDummyVerticesMap = new Dictionary<IEdge<IPositionedExtent>, IList<LayoutVertex>>();
        }

        public void Add(IPositionedExtent originalVertex)
        {
            var newLayoutVertex = CreateLayoutVertex(originalVertex);
            newLayoutVertex.CenterChanged += OnVertexCenterChanged;

            _originalToLayoutVertexMap.Add(originalVertex, newLayoutVertex);

            _layoutGraph.AddVertex(newLayoutVertex);
            _rankLayers.AddVertex(0, newLayoutVertex);
        }

        internal void Add(IEdge<IPositionedExtent> originalEdge)
        {
            // TODO: nem kell megfordítani az edge-et??

            var newLayoutEdge = CreateLayoutEdge(originalEdge);

            _layoutGraph.AddEdge(newLayoutEdge);
            _rankLayers.ArrangeVerticesOf(newLayoutEdge);

            OnEdgeRouteChanged(newLayoutEdge);
        }

        private void OnVertexCenterChanged(object sender, DiagramPoint diagramPoint)
        {
            var layoutVertex = (LayoutVertex)sender;

            if (!layoutVertex.IsDummy)
                VertexCenterChanged?.Invoke(layoutVertex.OriginalVertex, diagramPoint);

            foreach (var layoutEdge in _layoutGraph.GetAllEdges(layoutVertex))
                OnEdgeRouteChanged(layoutEdge);
        }

        private void OnEdgeRouteChanged(LayoutEdge layoutEdge)
        {
            var edgeRoute = GetEdgeRoute(layoutEdge).ToArray();
            EdgeRouteChanged?.Invoke(layoutEdge.OriginalEdge, edgeRoute);
        }

        private IEnumerable<DiagramPoint> GetEdgeRoute(LayoutEdge layoutEdge)
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

        private static LayoutVertex CreateLayoutVertex(IPositionedExtent originalVertex)
        {
            return LayoutVertex.Create(originalVertex);
        }

        private LayoutEdge CreateLayoutEdge(IEdge<IPositionedExtent> originalEdge)
        {
            var sourceLayoutVertex = _originalToLayoutVertexMap[originalEdge.Source];
            var targetLayoutVertex = _originalToLayoutVertexMap[originalEdge.Target];
            return new LayoutEdge(originalEdge, sourceLayoutVertex, targetLayoutVertex);
        }

        private void SaveEdgeToDummyVerticesMapping(LayoutEdge edge, IEnumerable<LayoutVertex> interimVertices)
        {
            interimVertices = edge.IsReversed ? interimVertices.Reverse() : interimVertices;

            _edgeToDummyVerticesMap.Add(edge.OriginalEdge, interimVertices.ToArray());
        }
    }
}
