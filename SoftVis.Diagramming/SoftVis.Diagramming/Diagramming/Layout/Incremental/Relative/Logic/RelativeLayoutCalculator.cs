using System;
using System.Collections.Generic;
using Codartis.SoftVis.Common;
using MoreLinq;

namespace Codartis.SoftVis.Diagramming.Layout.Incremental.Relative.Logic
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
    internal class RelativeLayoutCalculator : RelativeLayoutActionEventSource, IDiagramChangeConsumer
    {
        private readonly HighLevelLayoutGraph _highLevelLayoutGraph;
        private readonly LowLevelLayoutGraph _lowLevelLayoutGraph;
        private readonly LayoutVertexLayers _layers;
        private readonly RelativeLayout _relativeLayout;

        private readonly Map<DiagramNode, DiagramNodeLayoutVertex> _diagramNodeToLayoutVertexMap;
        private readonly Map<DiagramConnector, LayoutPath> _diagramConnectorToLayoutPathMap;
        private readonly RelativeLocationCalculator _locationCalculator;

        public RelativeLayoutCalculator()
        {
            _highLevelLayoutGraph = new HighLevelLayoutGraph();
            _lowLevelLayoutGraph = new LowLevelLayoutGraph();
            _layers = new LayoutVertexLayers();
            _relativeLayout = new RelativeLayout(_highLevelLayoutGraph, _lowLevelLayoutGraph, _layers);

            _diagramNodeToLayoutVertexMap = new Map<DiagramNode, DiagramNodeLayoutVertex>();
            _diagramConnectorToLayoutPathMap = new Map<DiagramConnector, LayoutPath>();
            _locationCalculator = new RelativeLocationCalculator(_relativeLayout);
        }

        public IReadOnlyRelativeLayout RelativeLayout => _relativeLayout;

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
            if (_diagramNodeToLayoutVertexMap.ContainsKey(diagramNode))
                throw new InvalidOperationException($"Diagram node {diagramNode} already added.");

            var diagramNodeLayoutVertex = new DiagramNodeLayoutVertex(diagramNode);
            _diagramNodeToLayoutVertexMap.Set(diagramNode, diagramNodeLayoutVertex);

            var location = AddDiagramNodeVertex(diagramNodeLayoutVertex);
            RaiseRelativeLocationAssignedLayoutAction(diagramNodeLayoutVertex, location, causingAction);

            CheckInvariants(diagramNodeLayoutVertex);
        }

        public void OnDiagramNodeRemoved(DiagramNode diagramNode, ILayoutAction causingAction)
        {
            if (!_diagramNodeToLayoutVertexMap.ContainsKey(diagramNode))
                throw new InvalidOperationException($"Diagram node {diagramNode} not found.");

            var diagramNodeLayoutVertex = _diagramNodeToLayoutVertexMap.Get(diagramNode);
            _diagramNodeToLayoutVertexMap.Remove(diagramNode);

            RemoveDiagramNodeVertex(diagramNodeLayoutVertex);
        }

        public void OnDiagramConnectorAdded(DiagramConnector diagramConnector, ILayoutAction causingAction)
        {
            if (_diagramConnectorToLayoutPathMap.ContainsKey(diagramConnector))
                throw new InvalidOperationException($"Diagram connector {diagramConnector} already added.");

            var layoutPath = CreateLayoutPath(diagramConnector);
            _diagramConnectorToLayoutPathMap.Set(diagramConnector, layoutPath);

            AddLayoutPath(layoutPath, causingAction);

            CheckInvariants(layoutPath.Vertices);
        }

        public void OnDiagramConnectorRemoved(DiagramConnector diagramConnector, ILayoutAction causingAction)
        {
            if (!_diagramConnectorToLayoutPathMap.ContainsKey(diagramConnector))
                throw new InvalidOperationException($"Diagram connector {diagramConnector} not found.");

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

        private RelativeLocation AddDiagramNodeVertex(DiagramNodeLayoutVertex vertex)
        {
            _highLevelLayoutGraph.AddVertex(vertex);
            _lowLevelLayoutGraph.AddVertex(vertex);

            var targetLocation = _locationCalculator.GetTargetLocation(vertex);
            _layers.AddVertex(vertex, targetLocation);

            return targetLocation;
        }

        private void RemoveDiagramNodeVertex(DiagramNodeLayoutVertex vertex)
        {
            _layers.RemoveVertex(vertex);
            _lowLevelLayoutGraph.RemoveVertex(vertex);
            _highLevelLayoutGraph.RemoveVertex(vertex);
        }

        private void AddLayoutPath(LayoutPath layoutPath, ILayoutAction causingAction)
        {
            _highLevelLayoutGraph.AddEdge(layoutPath);
            _lowLevelLayoutGraph.AddPath(layoutPath);

            EnsureCorrectLocationForPathSourceAndItsDescendants(layoutPath, causingAction);
        }

        private void RemoveLayoutPath(LayoutPath layoutPath)
        {
            _lowLevelLayoutGraph.RemovePath(layoutPath);

            foreach (var interimVertex in layoutPath.InterimVertices)
                _layers.RemoveVertex(interimVertex);

            _highLevelLayoutGraph.RemoveEdge(layoutPath);
        }

        private void EnsureCorrectLocationForPathSourceAndItsDescendants(LayoutPath layoutPath, ILayoutAction causingAction)
        {
            var movingVertex = layoutPath.PathSource;

            // Can't float tree here because floating parents' layer is undefined.
            //_lowLevelLayoutGraph.FloatTree(movingVertex);

            _highLevelLayoutGraph.ExecuteOnDescendantVertices(movingVertex, i => EnsureCorrectLocation(i, causingAction));
            _highLevelLayoutGraph.ExecuteOnDescendantVertices(movingVertex, i => AdjustPaths(i, causingAction));
        }

        private void EnsureCorrectLocation(LayoutVertexBase vertex, ILayoutAction causingAction)
        {
            var currentLocation = _layers.GetLocation(vertex);
            var targetLocation = _locationCalculator.GetTargetLocation(vertex);

            if (currentLocation != targetLocation)
                MoveVertex(vertex, targetLocation, causingAction);
        }

        private void MoveVertex(LayoutVertexBase vertex, RelativeLocation targetLocation, ILayoutAction causingAction)
        {
            var oldLocation = _layers.GetLocationOrThrow(vertex);

            _layers.RemoveVertex(vertex);
            _layers.AddVertex(vertex, targetLocation);

            RaiseRelativeLocationChangedLayoutAction(vertex, oldLocation, targetLocation, causingAction);
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
                EnsureCorrectLocation(dummyLayoutVertex, causingAction);
        }

        private void AdjustPathLength(LayoutPath layoutPath, ILayoutAction causingAction)
        {
            var sourceLayerIndex = _layers.GetLayerIndexOrThrow(layoutPath.Source);
            var targetLayerIndex = _layers.GetLayerIndexOrThrow(layoutPath.Target);

            var layerSpan = sourceLayerIndex - targetLayerIndex;
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
            _lowLevelLayoutGraph.AddVertex(interimVertex);
            _lowLevelLayoutGraph.AddEdge(newEdge1);
            _lowLevelLayoutGraph.AddEdge(newEdge2);

            var targetLocation = _locationCalculator.GetTargetLocation(interimVertex);
            _layers.AddVertex(interimVertex, targetLocation);

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

            _layers.RemoveVertex(vertexToRemove);

            _lowLevelLayoutGraph.RemoveEdge(firstEdge);
            _lowLevelLayoutGraph.RemoveEdge(nextEdge);
            _lowLevelLayoutGraph.RemoveVertex(vertexToRemove);
            _lowLevelLayoutGraph.AddEdge(mergedEdge);
        }

        private void CheckInvariants(IEnumerable<LayoutVertexBase> vertices)
        {
            vertices.ForEach(CheckInvariants);
        }

        private void CheckInvariants(LayoutVertexBase vertex)
        {
            var layerIndex = _layers.GetLayerIndexOrThrow(vertex);

            foreach (var parentVertex in _lowLevelLayoutGraph.GetParents(vertex))
                CheckLayerIndex(parentVertex, layerIndex - 1);

            foreach (var childVertex in _lowLevelLayoutGraph.GetChildren(vertex))
                CheckLayerIndex(childVertex, layerIndex + 1);

            foreach (var childVertex in _lowLevelLayoutGraph.GetSiblings(vertex))
                CheckLayerIndex(childVertex, layerIndex);
        }

        private void CheckLayerIndex(LayoutVertexBase vertex, int expectedLayerIndex)
        {
            var layerIndex = _layers.GetLayerIndex(vertex);
            if (layerIndex != expectedLayerIndex)
                throw new Exception($"Vertex {vertex} was expected to be on layer {expectedLayerIndex} but was on layer {layerIndex}.");
        }
    }
}