using System;
using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Common;
using Codartis.SoftVis.Geometry;
using MoreLinq;
using QuickGraph;

namespace Codartis.SoftVis.Diagramming.Layout.Incremental
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
    internal sealed class LayoutGraph : IBidirectionalGraph<LayoutVertex, LayoutEdge>
    {
        private readonly double _horizontalGap;
        private readonly double _verticalGap;
        private readonly BidirectionalGraph<LayoutVertex, LayoutEdge> _graph;
        private readonly DiagramLayers _diagramLayers;
        private readonly Dictionary<DiagramNode, LayoutVertex> _originalToLayoutVertexMap;
        private readonly IDictionary<DiagramConnector, IList<LayoutVertex>> _edgeToDummyVerticesMap;

        public event VertexAction<LayoutVertex> VertexAdded;
        public event VertexAction<LayoutVertex> VertexRemoved;
        public event EdgeAction<LayoutVertex, LayoutEdge> EdgeAdded;
        public event EdgeAction<LayoutVertex, LayoutEdge> EdgeRemoved;
        public event EventHandler Cleared;

        public LayoutGraph(double horizontalGap, double verticalGap)
        {
            _horizontalGap = horizontalGap;
            _verticalGap = verticalGap;

            _graph = new BidirectionalGraph<LayoutVertex, LayoutEdge>();
            SetUpGraphEventPropagation();
            _diagramLayers = new DiagramLayers(this, _verticalGap);

            _originalToLayoutVertexMap = new Dictionary<DiagramNode, LayoutVertex>();
            _edgeToDummyVerticesMap = new Dictionary<DiagramConnector, IList<LayoutVertex>>();
        }

        private void SetUpGraphEventPropagation()
        {
            _graph.VertexAdded += i => VertexAdded?.Invoke(i);
            _graph.VertexRemoved += i => VertexRemoved?.Invoke(i);
            _graph.EdgeAdded += i => EdgeAdded?.Invoke(i);
            _graph.EdgeRemoved += i => EdgeRemoved?.Invoke(i);
            _graph.Cleared += (i, j) => Cleared?.Invoke(i, j);
        }

        public LayoutVertex CreateVertex(DiagramNode diagramNode)
        {
            var newVertex = new LayoutVertex(this, diagramNode, isFloating: true);
            _originalToLayoutVertexMap.Add(diagramNode, newVertex);
            _graph.AddVertex(newVertex);
            return newVertex;
        }

        public LayoutVertex CreateDummyVertex()
        {
            var dummyVertex = new LayoutVertex(this, null, isFloating: true);
            _graph.AddVertex(dummyVertex);
            return dummyVertex;
        }

        public LayoutEdge CreateEdge(DiagramConnector diagramConnector)
        {
            var sourceLayoutVertex = _originalToLayoutVertexMap[diagramConnector.Source];
            var targetLayoutVertex = _originalToLayoutVertexMap[diagramConnector.Target];
            var newEdge = new LayoutEdge(this, sourceLayoutVertex, targetLayoutVertex, diagramConnector);
            _graph.AddEdge(newEdge);
            return newEdge;
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
            return _edgeToDummyVerticesMap.TryGetValue(layoutEdge.DiagramConnector, out dummyVertices)
                ? dummyVertices.Select(i => i.Center)
                : null;
        }

        public void Clear()
        {
            _originalToLayoutVertexMap.Clear();
            _edgeToDummyVerticesMap.Clear();
            _graph.Clear();
        }

        public DiagramLayer GetLayer(LayoutVertex layoutVertex) => _diagramLayers.GetLayer(layoutVertex);
        public int GetLayerIndex(LayoutVertex layoutVertex) => _diagramLayers.GetLayerIndex(layoutVertex);

        public IEnumerable<LayoutVertex> GetRootVertices() => _graph.Vertices.Where(i => _graph.OutDegree(i) == 0);

        #region Unused

        private void BreakEdgeWithInterimVertices(LayoutEdge edgeToBreak, List<LayoutVertex> interimVertices)
        {
            SaveEdgeToDummyVerticesMapping(edgeToBreak, interimVertices);

            _graph.RemoveEdge(edgeToBreak);

            _graph.AddVertexRange(interimVertices);

            var vertexPath = edgeToBreak.Source.ToEnumerable().Concat(interimVertices).Concat(edgeToBreak.Target).ToArray();

            for (var i = 0; i < vertexPath.Length - 1; i++)
            {
                var newEdge = new LayoutEdge(this, vertexPath[i], vertexPath[i + 1],
                    edgeToBreak.DiagramConnector, edgeToBreak.IsReversed);
                _graph.AddEdge(newEdge);
            }
        }

        private void SaveEdgeToDummyVerticesMapping(LayoutEdge edge, IEnumerable<LayoutVertex> interimVertices)
        {
            interimVertices = edge.IsReversed ? interimVertices.Reverse() : interimVertices;

            _edgeToDummyVerticesMap.Add(edge.DiagramConnector, interimVertices.ToArray());
        }

        #endregion

        public bool IsDirected => _graph.IsDirected;
        public bool AllowParallelEdges => _graph.AllowParallelEdges;

        public IEnumerable<LayoutVertex> Vertices => _graph.Vertices;
        public int VertexCount => _graph.VertexCount;
        public bool IsVerticesEmpty => _graph.IsVerticesEmpty;
        public bool ContainsVertex(LayoutVertex vertex) => _graph.ContainsVertex(vertex);

        public IEnumerable<LayoutEdge> Edges => _graph.Edges;
        public int EdgeCount => _graph.EdgeCount;
        public bool IsEdgesEmpty => _graph.IsEdgesEmpty;
        public bool IsInEdgesEmpty(LayoutVertex v) => _graph.IsInEdgesEmpty(v);
        public bool IsOutEdgesEmpty(LayoutVertex v) => _graph.IsOutEdgesEmpty(v);
        public IEnumerable<LayoutEdge> InEdges(LayoutVertex v) => _graph.InEdges(v);
        public IEnumerable<LayoutEdge> OutEdges(LayoutVertex v) => _graph.OutEdges(v);
        public LayoutEdge InEdge(LayoutVertex v, int index) => _graph.InEdge(v, index);
        public LayoutEdge OutEdge(LayoutVertex v, int index) => _graph.OutEdge(v, index);
        public bool TryGetEdges(LayoutVertex source, LayoutVertex target, out IEnumerable<LayoutEdge> edges) => _graph.TryGetEdges(source, target, out edges);
        public bool TryGetEdge(LayoutVertex source, LayoutVertex target, out LayoutEdge edge) => _graph.TryGetEdge(source, target, out edge);
        public bool TryGetInEdges(LayoutVertex v, out IEnumerable<LayoutEdge> edges) => _graph.TryGetInEdges(v, out edges);
        public bool TryGetOutEdges(LayoutVertex v, out IEnumerable<LayoutEdge> edges) => _graph.TryGetOutEdges(v, out edges);
        public bool ContainsEdge(LayoutVertex source, LayoutVertex target) => _graph.ContainsEdge(source, target);
        public bool ContainsEdge(LayoutEdge edge) => _graph.ContainsEdge(edge);

        public int Degree(LayoutVertex v) => _graph.Degree(v);
        public int InDegree(LayoutVertex v) => _graph.InDegree(v);
        public int OutDegree(LayoutVertex v) => _graph.OutDegree(v);
    }
}
