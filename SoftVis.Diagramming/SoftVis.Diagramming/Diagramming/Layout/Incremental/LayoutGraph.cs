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
        public event EventHandler<RectMove> LayoutVertexCenterChanged;

        private readonly double _horizontalGap;
        private readonly double _verticalGap;
        private readonly BidirectionalGraph<LayoutVertex, LayoutEdge> _graph;
        private readonly DiagramLayers _diagramLayers;
        private readonly Dictionary<DiagramNode, LayoutVertex> _originalToLayoutVertexMap;
        private readonly Dictionary<DiagramConnector, LayoutPath> _connectorToLayoutPath;

        public LayoutGraph(double horizontalGap, double verticalGap)
        {
            _horizontalGap = horizontalGap;
            _verticalGap = verticalGap;

            _graph = new BidirectionalGraph<LayoutVertex, LayoutEdge>();
            _diagramLayers = new DiagramLayers(_verticalGap);

            _originalToLayoutVertexMap = new Dictionary<DiagramNode, LayoutVertex>();
            _connectorToLayoutPath = new Dictionary<DiagramConnector, LayoutPath>();
        }

        public IEnumerable<DiagramLayer> Layers => _diagramLayers;

        public LayoutVertex AddNode(DiagramNode diagramNode)
        {
            var newVertex = new LayoutVertex(this, diagramNode, isFloating: true);
            SetUpEventPropagation(newVertex);
            AddVertex(newVertex, diagramNode);
            return newVertex;
        }

        public void RemoveNode(DiagramNode diagramNode)
        {
            if (!_originalToLayoutVertexMap.ContainsKey(diagramNode))
                return;

            RemoveVertex(_originalToLayoutVertexMap[diagramNode]);
            _originalToLayoutVertexMap.Remove(diagramNode);
        }

        public LayoutPath AddConnector(DiagramConnector diagramConnector)
        {
            // TODO: turn edge direction if needed!
            var isReversed = false;

            var sourceLayoutVertex = _originalToLayoutVertexMap[diagramConnector.Source];
            var targetLayoutVertex = _originalToLayoutVertexMap[diagramConnector.Target];

            var newEdge = new LayoutEdge(this, sourceLayoutVertex, targetLayoutVertex, diagramConnector);
            AddEdge(newEdge);

            var layerGap = newEdge.Source.GetLayerIndex() - newEdge.Target.GetLayerIndex();
            if (layerGap < 1)
                throw new Exception($"Invalid layerGap: {layerGap}");

            var layoutPath = (layerGap == 1)
                ? new LayoutPath(newEdge)
                : BreakEdgeWithDummyVertices(newEdge, layerGap - 1);

            SaveConnectorToLayoutPathMapping(diagramConnector, isReversed, layoutPath);
            return layoutPath;
        }

        public void RemoveConnector(DiagramConnector diagramConnector)
        {
            var layoutPath = _connectorToLayoutPath[diagramConnector];
            foreach (var layoutEdge in layoutPath)
            {
                _graph.RemoveEdge(layoutEdge);
                var sourceVertex = layoutEdge.Source;
                if (sourceVertex.IsDummy)
                {
                    _graph.RemoveVertex(sourceVertex);
                    _diagramLayers.RemoveVertex(sourceVertex);
                }
            }

            _connectorToLayoutPath.Remove(diagramConnector);
        }

        public LayoutPath GetPath(DiagramConnector diagramConnector)
        {
            return _connectorToLayoutPath[diagramConnector];
        }

        public void Clear()
        {
            _connectorToLayoutPath.Clear();
            _originalToLayoutVertexMap.Clear();
            _graph.Clear();
            _diagramLayers.Clear();
        }

        public IEnumerable<LayoutVertex> Vertices => _graph.Vertices;
        public IEnumerable<LayoutEdge> Edges => _graph.Edges;
        public int VertexCount => _graph.VertexCount;
        public int EdgeCount => _graph.EdgeCount;
        public int Degree(LayoutVertex v) => _graph.Degree(v);

        public IEnumerable<LayoutVertex> GetParents(LayoutVertex vertex) => _graph.GetOutNeighbours(vertex);
        public IEnumerable<LayoutVertex> GetChildren(LayoutVertex vertex) => _graph.GetInNeighbours(vertex);
        public IEnumerable<LayoutEdge> OutEdges(LayoutVertex vertex) => _graph.OutEdges(vertex);
        public IEnumerable<LayoutEdge> InEdges(LayoutVertex vertex) => _graph.InEdges(vertex);
        public IEnumerable<LayoutEdge> GetAllEdges(LayoutVertex vertex) => _graph.GetAllEdges(vertex);

        public IEnumerable<LayoutVertex> GetPositionedChildren(LayoutVertex layoutVertex)
        {
            return GetChildren(layoutVertex).Where(i => !i.IsFloating);
        }

        public IEnumerable<LayoutVertex> GetPrimarySiblings(LayoutVertex layoutVertex)
        {
            return GetPrimaryParent(layoutVertex)?.GetChildren().Where(i => i != layoutVertex);
        }

        public LayoutVertex GetPrimaryParent(LayoutVertex layoutVertex)
        {
            return GetOrderedParents(layoutVertex).FirstOrDefault();
        }

        private IEnumerable<LayoutVertex> GetOrderedParents(LayoutVertex layoutVertex)
        {
            var layerIndex = layoutVertex.GetLayerIndex();

            return GetParents(layoutVertex)
                .OrderByDescending(i => i.DiagramNode?.Priority ?? 0)
                .ThenBy(i => layerIndex - i.GetLayerIndex())
                .ThenBy(i => i.DiagramNode?.Name ?? string.Empty);
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

        public void TraverseInEdges(LayoutEdge edge, Action<LayoutEdge> action)
        {
            _graph.TraverseEdges(edge, EdgeDirection.In, action);
        }

        public DiagramLayer GetLayer(LayoutVertex layoutVertex) => _diagramLayers.GetLayer(layoutVertex);
        public int GetLayerIndex(LayoutVertex layoutVertex) => _diagramLayers.GetLayerIndex(layoutVertex);

        private IEnumerable<LayoutVertex> CreateDummyVertices(int count)
        {
            for (var i = 0; i < count; i++)
                yield return new LayoutVertex(this, null, isFloating: true);
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
            newEdge.TraverseInEdges(i => EnsureVertexIsUnderParentVertex(i.Source, i.Target));
        }

        private void RemoveEdge(LayoutEdge layoutEdge)
        {
            _graph.RemoveEdge(layoutEdge);
        }

        private void EnsureVertexIsUnderParentVertex(LayoutVertex childVertex, LayoutVertex parentVertex)
        {
            var childVertexLayerIndex = GetLayerIndex(childVertex);
            var parentVertexLayerIndex = GetLayerIndex(parentVertex);

            if (childVertexLayerIndex > parentVertexLayerIndex)
                EnsureItemIndexIsValid(childVertex);
            else
                PushVertexDownToLayer(childVertex, parentVertexLayerIndex + 1);
        }

        private void PushVertexDownToLayer(LayoutVertex layoutVertex, int targetLayer)
        {
            while (layoutVertex.GetLayerIndex() < targetLayer)
                PushVertexDownOneLayer(layoutVertex);
        }

        private void PushVertexDownOneLayer(LayoutVertex layoutVertex)
        {
            var currentLayer = layoutVertex.GetLayerIndex();
            _diagramLayers.MoveVertexBetweenLayers(layoutVertex, currentLayer, currentLayer+1);

            foreach (var inEdge in layoutVertex.InEdges().ToList())
            {
                var sourceVertex = inEdge.Source;
                if (sourceVertex.IsDummy)
                {
                    var path = _connectorToLayoutPath[inEdge.DiagramConnector];
                    var modifiedPath = RemoveVertexFromPath(sourceVertex, path);
                    _connectorToLayoutPath[inEdge.DiagramConnector] = modifiedPath;
                    RemoveVertex(sourceVertex);
                }
            }
        }

        private void RemoveVertex(LayoutVertex layoutVertex)
        {
            _graph.RemoveVertex(layoutVertex);
            _diagramLayers.RemoveVertex(layoutVertex);
        }

        private LayoutPath RemoveVertexFromPath(LayoutVertex layoutVertex, LayoutPath layoutPath)
        {
            var edgesInPath = layoutPath.ToList();

            for (var i = 1; i < edgesInPath.Count; i++)
            {
                var currentEdge = edgesInPath[i];
                if (currentEdge.Source == layoutVertex)
                {
                    var previousEdge = edgesInPath[i - 1];

                    var mergedEdge = new LayoutEdge(this, previousEdge.Source, currentEdge.Target,
                        currentEdge.DiagramConnector, currentEdge.IsReversed);

                    AddEdge(mergedEdge);
                    edgesInPath.Insert(i, mergedEdge);

                    RemoveEdge(previousEdge);
                    edgesInPath.Remove(previousEdge);

                    RemoveEdge(currentEdge);
                    edgesInPath.Remove(currentEdge);

                    break;
                }
            }

            return new LayoutPath(edgesInPath);
        }

        private static void EnsureItemIndexIsValid(LayoutVertex layoutVertex)
        {
            if (!layoutVertex.IsLayerItemIndexValid)
                layoutVertex.RearrangeItemInLayer();
        }

        private LayoutPath BreakEdgeWithDummyVertices(LayoutEdge edgeToBreak, int dummyVertexCount)
        {
            _graph.RemoveEdge(edgeToBreak);

            var dummyVertices = CreateDummyVertices(dummyVertexCount).ToArray();
            dummyVertices.ForEach(i => AddVertex(i));

            var vertices = edgeToBreak.Source.ToEnumerable()
                .Concat(dummyVertices)
                .Concat(edgeToBreak.Target).ToArray();

            var diagramConnector = edgeToBreak.DiagramConnector;
            var isReversed = edgeToBreak.IsReversed;

            var edges = new List<LayoutEdge>();
            for (var i = 0; i < vertices.Length - 1; i++)
            {
                var newEdge = new LayoutEdge(this, vertices[i], vertices[i + 1], diagramConnector, isReversed);
                AddEdge(newEdge);
                edges.Add(newEdge);
            }

            return new LayoutPath(edges);
        }

        private void SaveConnectorToLayoutPathMapping(DiagramConnector diagramConnector, bool isReversed,
            LayoutPath layoutPath)
        {
            // TODO: reverse stored path?
            //IEnumerable<LayoutEdge> edges = isReversed ? layoutPath.Reverse() : layoutPath;
            _connectorToLayoutPath.Add(diagramConnector, layoutPath);
        }

        private void SetUpEventPropagation(LayoutVertex layoutVertex)
        {
            layoutVertex.CenterChanged += (o, a) => LayoutVertexCenterChanged?.Invoke(o, a);
        }
    }
}
