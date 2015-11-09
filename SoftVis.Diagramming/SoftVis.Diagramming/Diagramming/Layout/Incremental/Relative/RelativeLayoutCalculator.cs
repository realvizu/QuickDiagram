using System;
using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Common;
using Codartis.SoftVis.Diagramming.Layout.ActionTracking;
using Codartis.SoftVis.Diagramming.Layout.Incremental.ActionTracking;

namespace Codartis.SoftVis.Diagramming.Layout.Incremental.Relative
{
    /// <summary>
    /// Calculates the arrangement of layout vertices relative to each other.
    /// </summary>
    /// <remarks>
    /// Arranges vertices into layers so that:
    /// <para>all edges point "upward" (to a layer with lower index),</para>
    /// <para>all edges span exactly 2 layers (by using dummy vertices as necessary)</para>
    /// <para>vertices in all layers ar ordered so that primary edges never cross.</para>
    /// No primary edge crossing is achieved by:
    /// <para>dummy vertices are inserted to ensure that parent and children are always on adjacent layers,</para>
    /// <para>primary siblings on a layer are always placed together to form a block,</para>
    /// <para>if a vertex has no primary siblings on its layer but has a primary parent 
    /// then it is ordered according to parent ordering.</para>
    /// </remarks>
    internal class RelativeLayoutCalculator : IncrementalLayoutActionEventSource,
        IDiagramChangeConsumer, IReadOnlyRelativeLayout
    {
        private readonly HighLevelLayoutGraph _highLevelLayoutGraph;
        private readonly LowLevelLayoutGraph _lowLevelLayoutGraph;
        private readonly LayoutVertexLayers _layers;
        private readonly IComparer<LayoutVertexBase> _vertexComparer;

        private readonly Map<DiagramNode, DiagramNodeLayoutVertex> _diagramNodeToLayoutVertexMap;
        private readonly Map<DiagramConnector, LayoutPath> _diagramConnectorToLayoutPathMap;

        public RelativeLayoutCalculator()
        {
            _highLevelLayoutGraph = new HighLevelLayoutGraph();
            _lowLevelLayoutGraph = new LowLevelLayoutGraph();
            _layers = new LayoutVertexLayers();
            _vertexComparer = new VerticesInLayerComparer(_lowLevelLayoutGraph);

            _diagramNodeToLayoutVertexMap = new Map<DiagramNode, DiagramNodeLayoutVertex>();
            _diagramConnectorToLayoutPathMap = new Map<DiagramConnector, LayoutPath>();
        }

        public IReadOnlyHighLevelLayoutGraph HighLevelLayoutGraph => _highLevelLayoutGraph;
        public IReadOnlyLowLevelLayoutGraph LowLevelLayoutGraph => _lowLevelLayoutGraph;
        public IReadOnlyLayoutVertexLayers LayoutVertexLayers => _layers;

        public void OnDiagramCleared()
        {
            _layers.Clear();
            _lowLevelLayoutGraph.Clear();
            _highLevelLayoutGraph.Clear();

            _diagramNodeToLayoutVertexMap.Clear();
            _diagramConnectorToLayoutPathMap.Clear();
        }

        public void OnDiagramNodeAdded(DiagramNode diagramNode, ILayoutAction causingAction)
        {
            var diagramNodeLayoutVertex = new DiagramNodeLayoutVertex(diagramNode);
            _diagramNodeToLayoutVertexMap.Set(diagramNode, diagramNodeLayoutVertex);

            AddVertex(diagramNodeLayoutVertex);
            var to = GetRelativeLocation(diagramNodeLayoutVertex);

            var layoutAction = new RelativeVertexAddLayoutAction(diagramNodeLayoutVertex, to, causingAction);
            RaiseLayoutAction(this, layoutAction);
        }

        private RelativeLocation GetRelativeLocation(DiagramNodeLayoutVertex diagramNodeLayoutVertex)
        {
            return new RelativeLocation(_layers.GetLayerIndex(diagramNodeLayoutVertex),
                _layers.GetIndexInLayer(diagramNodeLayoutVertex));
        }

        public void OnDiagramNodeRemoved(DiagramNode diagramNode, ILayoutAction causingAction)
        {
            var diagramNodeLayoutVertex = _diagramNodeToLayoutVertexMap.Get(diagramNode);
            _diagramNodeToLayoutVertexMap.Remove(diagramNode);

            RemoveVertex(diagramNodeLayoutVertex);
        }

        public void OnDiagramConnectorAdded(DiagramConnector diagramConnector, ILayoutAction causingAction)
        {
            var layoutPath = CreateLayoutPath(diagramConnector);
            _diagramConnectorToLayoutPathMap.Set(diagramConnector, layoutPath);

            AddLayoutPath(layoutPath, causingAction);
        }

        public void OnDiagramConnectorRemoved(DiagramConnector diagramConnector, ILayoutAction causingAction)
        {
            var layoutPath = _diagramConnectorToLayoutPathMap.Get(diagramConnector);
            _diagramConnectorToLayoutPathMap.Remove(diagramConnector);

            RemoveLayoutPath(layoutPath);
        }

        private LayoutPath CreateLayoutPath(DiagramConnector diagramConnector)
        {
            var sourceVertex = _diagramNodeToLayoutVertexMap.Get(diagramConnector.Source);
            var targetVertex = _diagramNodeToLayoutVertexMap.Get(diagramConnector.Target);
            var newEdge = new LayoutEdge(sourceVertex, targetVertex, diagramConnector);
            var layoutPath = new LayoutPath(newEdge);
            return layoutPath;
        }

        private void AddVertex(LayoutVertexBase vertex)
        {
            _lowLevelLayoutGraph.AddVertex(vertex);

            var targetLayerIndex = CalculateRankInLowLevelLayoutGraph(vertex);
            var indexInLayer = DetermineIndexInLayer(vertex, targetLayerIndex);
            _layers.AddVertex(vertex, targetLayerIndex, indexInLayer);
        }

        private void RemoveVertex(LayoutVertexBase vertex)
        {
            _layers.RemoveVertex(vertex);
            _lowLevelLayoutGraph.RemoveVertex(vertex);
        }

        private void AddLayoutPath(LayoutPath layoutPath, ILayoutAction causingAction)
        {
            _lowLevelLayoutGraph.AddPath(layoutPath);

            EnsureCorrectLocationForPathSourceAndItsDescendants(layoutPath, causingAction);
        }

        private void RemoveLayoutPath(LayoutPath layoutPath)
        {
            _lowLevelLayoutGraph.RemovePath(layoutPath);

            foreach (var interimVertex in layoutPath.InterimVertices)
                _layers.RemoveVertex(interimVertex);
        }

        private void EnsureCorrectLocationForPathSourceAndItsDescendants(LayoutPath layoutPath, ILayoutAction causingAction)
        {
            var movingVertex = layoutPath.PathSource;
            _lowLevelLayoutGraph.FloatTree(movingVertex);

            _highLevelLayoutGraph.ExecuteOnDescendantVertices(movingVertex,
                i => EnsureCorrectLocationForVertex(i, causingAction));
        }

        private void EnsureCorrectLocationForVertex(LayoutVertexBase vertex, ILayoutAction causingAction)
        {
            var isDiagramNodeLayoutVertex = vertex is DiagramNodeLayoutVertex;
            var currentLayerIndex = _layers.GetLayerIndex(vertex);
            var currentIndexInLayer = _layers.GetIndexInLayer(vertex);

            var minimumLayerIndex = isDiagramNodeLayoutVertex
                ? CalculateRankInHighLevelLayoutGraph(vertex)
                : CalculateRankInLowLevelLayoutGraph(vertex);

            var toLayerIndex = Math.Max(currentLayerIndex, minimumLayerIndex);
            var toIndexInLayer = DetermineIndexInLayer(vertex, minimumLayerIndex);

            if (currentLayerIndex != toLayerIndex || currentIndexInLayer != toIndexInLayer)
            {
                MoveVertex(vertex, toLayerIndex, toIndexInLayer, causingAction);

                if (isDiagramNodeLayoutVertex)
                    AdjustPaths((DiagramNodeLayoutVertex)vertex, causingAction);
            }
        }

        private int CalculateRankInLowLevelLayoutGraph(LayoutVertexBase vertex)
        {
            return _lowLevelLayoutGraph.GetRank(vertex, i => _layers.GetLayerIndex(i));
        }

        private int CalculateRankInHighLevelLayoutGraph(LayoutVertexBase vertex)
        {
            return _highLevelLayoutGraph.GetRank((DiagramNodeLayoutVertex)vertex, i => _layers.GetLayerIndex(i));
        }

        private int DetermineIndexInLayer(LayoutVertexBase vertex, int layerIndex)
        {
            var layer = _layers.GetLayer(layerIndex);

            var parentVertex = _lowLevelLayoutGraph.GetPrimaryParent(vertex);
            if (parentVertex == null)
                return layer.Count;

            var siblingsInLayer = GetPrimarySiblingsInSameLayer(vertex).OrderBy(layer.IndexOf).ToArray();
            if (siblingsInLayer.Any())
                return CalculateInsertionIndexBasedOnSiblings(vertex, siblingsInLayer);

            return CalculateInsertionIndexBasedOnParents(vertex, parentVertex);
        }

        private int CalculateInsertionIndexBasedOnSiblings(LayoutVertexBase vertex, LayoutVertexBase[] siblingsInLayer)
        {
            var followingSiblingInLayer = siblingsInLayer.FirstOrDefault(i => Precedes(vertex, i));
            return followingSiblingInLayer != null
                ? _layers.GetIndexInLayer(followingSiblingInLayer)
                : _layers.GetIndexInLayer(siblingsInLayer.Last()) + 1;
        }

        private int CalculateInsertionIndexBasedOnParents(LayoutVertexBase vertex, LayoutVertexBase parentVertex)
        {
            CheckThatParentIsAtOneLayerHigherThanVertex(vertex, parentVertex);

            var parentLayer = _layers.GetLayer(parentVertex);
            var parentIndexInLayer = _layers.GetIndexInLayer(parentVertex);

            var followingParent = GetFollowingVerticesWithPrimaryChildren(parentLayer, parentIndexInLayer).FirstOrDefault();
            if (followingParent == null)
                return _layers.GetLayer(vertex).Count;

            var firstChildOfFollowingParent = _lowLevelLayoutGraph.GetPrimaryChildren(followingParent)
                .OrderBy(_layers.GetIndexInLayer).First();
            CheckThatVerticesAreOnTheSameLayer(vertex, firstChildOfFollowingParent);
            return _layers.GetIndexInLayer(firstChildOfFollowingParent);
        }

        private IEnumerable<LayoutVertexBase> GetFollowingVerticesWithPrimaryChildren(
            IReadOnlyLayoutVertexLayer layer, int index)
        {
            return layer.OrderBy(_layers.GetIndexInLayer)
                .Where(i => _layers.GetIndexInLayer(i) > index && _lowLevelLayoutGraph.HasPrimaryChildren(i));
        }

        private void CheckThatParentIsAtOneLayerHigherThanVertex(LayoutVertexBase vertex, LayoutVertexBase parentVertex)
        {
            var layerIndex = _layers.GetLayerIndex(vertex);
            var parentLayerIndex = _layers.GetLayerIndex(parentVertex);
            if (layerIndex != parentLayerIndex + 1)
                throw new Exception($"Child was expected to be 1 layer lower than parent, but vertex {vertex} is on layer {layerIndex} and parent {parentVertex} is on layer {parentLayerIndex}.");
        }

        private void CheckThatVerticesAreOnTheSameLayer(LayoutVertexBase vertex1, LayoutVertexBase vertex2)
        {
            var layerIndex1 = _layers.GetLayerIndex(vertex1);
            var layerIndex2 = _layers.GetLayerIndex(vertex2);
            if (layerIndex1 != layerIndex2)
                throw new Exception($"Vertices were expected to be on the same layer, but vertex {vertex1} is on layer {layerIndex1} and vertex {vertex2} is on layer {layerIndex2}.");
        }

        private bool Precedes(LayoutVertexBase vertex1, LayoutVertexBase vertex2)
        {
            return _vertexComparer.Compare(vertex1, vertex2) < 0;
        }

        private void MoveVertex(LayoutVertexBase vertex, int toLayerIndex, int toIndexInLayer,
            ILayoutAction causingAction)
        {
            var from = new RelativeLocation(_layers.GetLayerIndex(vertex), _layers.GetIndexInLayer(vertex));
            var to = new RelativeLocation(toLayerIndex, toIndexInLayer);

            _layers.RemoveVertex(vertex);
            _layers.AddVertex(vertex, toLayerIndex, toIndexInLayer);

            var layoutAction = new RelativeVertexMoveLayoutAction(vertex, from, to, causingAction);
            RaiseLayoutAction(this, layoutAction);
        }

        private void AdjustPaths(DiagramNodeLayoutVertex diagramNodeLayoutVertex, ILayoutAction causingAction)
        {
            var outPaths = _highLevelLayoutGraph.OutEdges(diagramNodeLayoutVertex);
            foreach (var outPath in outPaths)
                AdjustPath(outPath, causingAction);
        }

        private void AdjustPath(LayoutPath layoutPath, ILayoutAction causingAction)
        {
            AdjustPathLength(layoutPath, causingAction);
            EnsureCorrectLocationForInterimVertices(layoutPath, causingAction);
        }

        private void EnsureCorrectLocationForInterimVertices(LayoutPath layoutPath, ILayoutAction causingAction)
        {
            foreach (var dummyLayoutVertex in layoutPath.InterimVertices)
                EnsureCorrectLocationForVertex(dummyLayoutVertex, causingAction);
        }

        private void AdjustPathLength(LayoutPath layoutPath, ILayoutAction causingAction)
        {
            var layerSpan = _layers.GetLayerIndex(layoutPath.Source) - _layers.GetLayerIndex(layoutPath.Target);
            var pathLengthDifference = layerSpan - layoutPath.Length;

            if (pathLengthDifference > 0)
                SplitEdge(layoutPath, 0, pathLengthDifference, causingAction);
            else if (pathLengthDifference < 0)
                MergeEdgeWithNext(layoutPath, 0, -pathLengthDifference, causingAction);
        }

        private void SplitEdge(LayoutPath layoutPath, int atIndex, int times, ILayoutAction causingAction)
        {
            for (var i = 0; i < times; i++)
                SplitEdge(layoutPath, atIndex, causingAction);
        }

        private void SplitEdge(LayoutPath layoutPath, int atIndex, ILayoutAction causingAction)
        {
            var edgeToSplit = layoutPath[atIndex];
            var interimVertex = new DummyLayoutVertex(true);
            var newEdge1 = new LayoutEdge(edgeToSplit.Source, interimVertex, edgeToSplit.DiagramConnector);
            var newEdge2 = new LayoutEdge(interimVertex, edgeToSplit.Target, edgeToSplit.DiagramConnector);

            layoutPath.Substitute(atIndex, 1, newEdge1, newEdge2);

            _lowLevelLayoutGraph.RemoveEdge(edgeToSplit);
            AddVertex(interimVertex);
            _lowLevelLayoutGraph.AddEdge(newEdge1);
            _lowLevelLayoutGraph.AddEdge(newEdge2);

            RaiseVertexLayoutAction("DummyVertexCreated", interimVertex, causingAction);
        }

        private void MergeEdgeWithNext(LayoutPath layoutPath, int atIndex, int times, ILayoutAction causingAction)
        {
            for (var i = 0; i < times; i++)
                MergeEdgeWithNext(layoutPath, atIndex, causingAction);
        }

        private void MergeEdgeWithNext(LayoutPath layoutPath, int atIndex, ILayoutAction causingAction)
        {
            var firstEdge = layoutPath[atIndex];
            var nextEdge = layoutPath[atIndex + 1];
            var vertexToRemove = firstEdge.Target as DummyLayoutVertex;
            var mergedEdge = new LayoutEdge(firstEdge.Source, nextEdge.Target, firstEdge.DiagramConnector);

            if (vertexToRemove == null)
                throw new Exception("FirstEdge.Target is null or not dummy!");

            RaiseVertexLayoutAction("DummyVertexRemoved", vertexToRemove, causingAction);

            layoutPath.Substitute(atIndex, 2, mergedEdge);

            _lowLevelLayoutGraph.RemoveEdge(firstEdge);
            _lowLevelLayoutGraph.RemoveEdge(nextEdge);
            RemoveVertex(vertexToRemove);
            _lowLevelLayoutGraph.AddEdge(mergedEdge);
        }

        private IEnumerable<LayoutVertexBase> GetPrimarySiblingsInSameLayer(LayoutVertexBase vertex)
        {
            var layerIndex = _layers.GetLayerIndex(vertex);
            return _lowLevelLayoutGraph.GetPrimarySiblings(vertex).Where(i => _layers.GetLayerIndex(i) == layerIndex);
        }

        private IEnumerable<LayoutVertexBase> GetPlacedPrimarySiblingsInSameLayer(LayoutVertexBase vertex)
        {
            return GetPrimarySiblingsInSameLayer(vertex).Where(i => !i.IsFloating);
        }

        public bool HasPlacedPrimarySiblingsInSameLayer(LayoutVertexBase vertex)
        {
            return GetPlacedPrimarySiblingsInSameLayer(vertex).Any();
        }

        public LayoutVertexBase GetPreviousPlacedPrimarySiblingInSameLayer(LayoutVertexBase vertex)
        {
            var previousVertex = _layers.GetPreviousInLayer(vertex);
            return _lowLevelLayoutGraph.IsPlacedPrimarySiblingOf(vertex, previousVertex) ? previousVertex : null;
        }

        public LayoutVertexBase GetNextPlacedPrimarySiblingInSameLayer(LayoutVertexBase vertex)
        {
            var nextVertex = _layers.GetNextInLayer(vertex);
            return _lowLevelLayoutGraph.IsPlacedPrimarySiblingOf(vertex, nextVertex) ? nextVertex : null;
        }

    }
}