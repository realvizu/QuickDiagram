using System;
using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Common;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Graphs;
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
    internal sealed class LayoutGraph
    {
        private readonly double _horizontalGap;
        private readonly double _verticalGap;
        private readonly BidirectionalGraph<LayoutVertex, LayoutEdge> _graph;
        private readonly DiagramLayers _diagramLayers;
        private readonly Dictionary<DiagramNode, LayoutVertex> _originalToLayoutVertexMap;
        private readonly IDictionary<DiagramConnector, IList<LayoutVertex>> _edgeToDummyVerticesMap;

        public LayoutGraph(double horizontalGap, double verticalGap)
        {
            _horizontalGap = horizontalGap;
            _verticalGap = verticalGap;

            _graph = new BidirectionalGraph<LayoutVertex, LayoutEdge>();
            _diagramLayers = new DiagramLayers(_verticalGap);

            _originalToLayoutVertexMap = new Dictionary<DiagramNode, LayoutVertex>();
            _edgeToDummyVerticesMap = new Dictionary<DiagramConnector, IList<LayoutVertex>>();
        }

        public IEnumerable<DiagramLayer> Layers => _diagramLayers;

        public LayoutVertex CreateVertex(DiagramNode diagramNode)
        {
            var newVertex = new LayoutVertex(this, diagramNode, isFloating: true);
            AddVertex(newVertex, diagramNode);
            return newVertex;
        }

        public LayoutVertex CreateDummyVertex()
        {
            var dummyVertex = new LayoutVertex(this, null, isFloating: true);
            AddVertex(dummyVertex);
            return dummyVertex;
        }

        public LayoutEdge CreateEdge(DiagramConnector diagramConnector)
        {
            var sourceLayoutVertex = _originalToLayoutVertexMap[diagramConnector.Source];
            var targetLayoutVertex = _originalToLayoutVertexMap[diagramConnector.Target];
            var newEdge = new LayoutEdge(this, sourceLayoutVertex, targetLayoutVertex, diagramConnector);
            AddEdge(newEdge);
            return newEdge;
        }

        public void Clear()
        {
            _originalToLayoutVertexMap.Clear();
            _edgeToDummyVerticesMap.Clear();
            _graph.Clear();
            _diagramLayers.Clear();
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

        public IEnumerable<LayoutVertex> Vertices => _graph.Vertices;
        public IEnumerable<LayoutEdge> Edges => _graph.Edges;
        public int VertexCount => _graph.VertexCount;
        public int EdgeCount => _graph.EdgeCount;
        public int Degree(LayoutVertex v) => _graph.Degree(v);

        public IEnumerable<LayoutVertex> GetParents(LayoutVertex layoutVertex) => _graph.GetOutNeighbours(layoutVertex);
        public IEnumerable<LayoutVertex> GetChildren(LayoutVertex layoutVertex) => _graph.GetInNeighbours(layoutVertex);
        public IEnumerable<LayoutEdge> GetAllEdges(LayoutVertex layoutVertex) => _graph.GetAllEdges(layoutVertex);

        public IEnumerable<LayoutVertex> GetPositionedChildren(LayoutVertex layoutVertex)
        {
            return GetChildren(layoutVertex).Where(i => !i.IsFloating);
        }

        public IEnumerable<LayoutVertex> GetSiblings(LayoutVertex layoutVertex)
        {
            return GetParents(layoutVertex).SelectMany(i => i.GetChildren()).Where(i => i != layoutVertex);
        }

        public LayoutVertex GetPrimaryParent(LayoutVertex layoutVertex)
        {
            return GetOrderedParents(layoutVertex).FirstOrDefault();
        }

        private IEnumerable<LayoutVertex> GetOrderedParents(LayoutVertex layoutVertex)
        {
            return GetParents(layoutVertex)
                .OrderByDescending(i => i.DiagramNode?.Priority)
                .ThenBy(i => i.DiagramNode?.Name);
        }

        public IEnumerable<LayoutVertex> GetNonPrimaryParents(LayoutVertex layoutVertex)
        {
            return GetOrderedParents(layoutVertex).Skip(1);
        }

        public void ExecuteOnPrimaryTree(LayoutVertex rootVertex, Action<LayoutVertex> actionOnVertex)
        {
            actionOnVertex(rootVertex);
            foreach (var layoutEdge in GetPrimaryEdges(rootVertex))
            {
                var nextVertex = layoutEdge.Source;
                ExecuteOnPrimaryTree(nextVertex, actionOnVertex);
            }
        }

        private IEnumerable<LayoutEdge> GetPrimaryEdges(LayoutVertex layoutVertex)
        {
            return _graph.InEdges(layoutVertex).Where(i => i.Source.GetPrimaryParent() == layoutVertex);
        }

        public void ExecuteOnTree(LayoutEdge edge, Action<LayoutEdge> action)
        {
            _graph.ExecuteOnTree(edge, EdgeDirection.In, action);
        }

        public DiagramLayer GetLayer(LayoutVertex layoutVertex) => _diagramLayers.GetLayer(layoutVertex);
        public int GetLayerIndex(LayoutVertex layoutVertex) => _diagramLayers.GetLayerIndex(layoutVertex);

        private IEnumerable<Point2D> GetInterimRoutePoints(LayoutEdge layoutEdge)
        {
            IList<LayoutVertex> dummyVertices;
            return _edgeToDummyVerticesMap.TryGetValue(layoutEdge.DiagramConnector, out dummyVertices)
                ? dummyVertices.Select(i => i.Center)
                : null;
        }

        private void AddVertex(LayoutVertex newVertex, DiagramNode diagramNode = null)
        {
            if (diagramNode != null)
                _originalToLayoutVertexMap.Add(diagramNode, newVertex);

            _graph.AddVertex(newVertex);
            _diagramLayers.AddVertex(newVertex);
        }

        private void AddEdge(LayoutEdge newEdge)
        {
            _graph.AddEdge(newEdge);
            newEdge.ExecuteOnTree(i => _diagramLayers.EnsureVertexIsUnderParentVertex(i.Source, i.Target));
        }

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
    }
}
