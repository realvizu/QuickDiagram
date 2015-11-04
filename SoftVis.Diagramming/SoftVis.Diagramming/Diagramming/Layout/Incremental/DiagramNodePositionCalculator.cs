using System;
using System.Linq;
using Codartis.SoftVis.Common;
using Codartis.SoftVis.Diagramming.Layout.ActionTracking;
using Codartis.SoftVis.Diagramming.Layout.Incremental.ActionTracking;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Graphs;

namespace Codartis.SoftVis.Diagramming.Layout.Incremental
{
    /// <summary>
    /// Calculates diagram node positions and diagram connector routes when they are added or removed.
    /// </summary>
    internal class DiagramNodePositionCalculator : IncrementalLayoutActionEventSource, IDiagramChangeConsumer
    {
        private readonly IReadOnlyDiagramGraph _diagramGraph;
        private readonly IDiagramNodeRankProvider _diagramNodeRankProvider;

        private readonly LayoutGraph _layoutGraph;
        private readonly LayoutVertexLayers _layers;
        private readonly Map<DiagramNode, DiagramNodeLayoutVertex> _diagramNodeToLayoutVertexMap;
        private readonly Map<DiagramConnector, LayoutPath> _diagramConnectorToLayoutPathMap;
        private readonly Map<LayoutPath, Route> _layoutPathToPreviousRouteMap;

        private readonly VertexPositioningLogic _vertexPositioningLogic;

        public DiagramNodePositionCalculator(double horizontalGap, double verticalGap,
            IReadOnlyDiagramGraph diagramGraph, IDiagramNodeRankProvider diagramNodeRankProvider)
        {
            _diagramGraph = diagramGraph;
            _diagramNodeRankProvider = diagramNodeRankProvider;

            _layoutGraph = new LayoutGraph();
            _layers = new LayoutVertexLayers(_layoutGraph, verticalGap);
            _diagramNodeToLayoutVertexMap = new Map<DiagramNode, DiagramNodeLayoutVertex>();
            _diagramConnectorToLayoutPathMap = new Map<DiagramConnector, LayoutPath>();
            _layoutPathToPreviousRouteMap = new Map<LayoutPath, Route>();

            _vertexPositioningLogic = new VertexPositioningLogic(horizontalGap, verticalGap, 
                _layoutGraph, _layers);
            _vertexPositioningLogic.LayoutActionExecuted += OnLayoutActionExecuted;
        }

        public void Clear()
        {
            _layers.Clear();
            _layoutGraph.Clear();
            _diagramNodeToLayoutVertexMap.Clear();
            _diagramConnectorToLayoutPathMap.Clear();
            _layoutPathToPreviousRouteMap.Clear();
        }

        public void Add(DiagramNode diagramNode)
        {
            var layoutAction = RaiseDiagramNodeLayoutAction("AddNode", diagramNode);

            var diagramNodeVertex = new DiagramNodeLayoutVertex(diagramNode);
            _diagramNodeToLayoutVertexMap.Set(diagramNode, diagramNodeVertex);

            _layoutGraph.AddVertex(diagramNodeVertex);
            _layers.AddVertex(diagramNodeVertex);

            _vertexPositioningLogic.PositionVertex(diagramNodeVertex, layoutAction);
            _vertexPositioningLogic.Compact(layoutAction);
        }

        public void Remove(DiagramNode diagramNode)
        {
            var layoutAction = RaiseDiagramNodeLayoutAction("RemoveNode", diagramNode);

            var layoutVertex = _diagramNodeToLayoutVertexMap.Get(diagramNode);

            _vertexPositioningLogic.CoverUpVertex(layoutVertex, layoutAction);

            _layers.RemoveVertex(layoutVertex);
            _layoutGraph.RemoveVertex(layoutVertex);

            _diagramNodeToLayoutVertexMap.Remove(diagramNode);

            _vertexPositioningLogic.Compact(layoutAction);
        }

        public void Add(DiagramConnector diagramConnector)
        {
            var layoutAction = RaiseDiagramConnectorLayoutAction("AddConnector", diagramConnector);

            var sourceVertex = _diagramNodeToLayoutVertexMap.Get(diagramConnector.Source);
            var targetVertex = _diagramNodeToLayoutVertexMap.Get(diagramConnector.Target);
            var newEdge = new LayoutEdge(sourceVertex, targetVertex, diagramConnector);
            var newPath = new LayoutPath(newEdge);
            _diagramConnectorToLayoutPathMap.Set(diagramConnector, newPath);
            _layoutGraph.AddPath(newPath);
            //TODO: add to layers

            AdjustPathsToLayerSpansRecursive(diagramConnector.Source, layoutAction);
            PositionVerticesOnModifiedPathsRecursive(diagramConnector.Source, layoutAction);

            _vertexPositioningLogic.Compact(layoutAction);

            ReroutePath(newPath, layoutAction);
        }

        public void Remove(DiagramConnector diagramConnector)
        {
            var layoutAction = RaiseDiagramConnectorLayoutAction("RemoveConnector", diagramConnector);

            var layoutPath = _diagramConnectorToLayoutPathMap.Get(diagramConnector);

            foreach (var interimVertex in layoutPath.InterimVertices)
                _vertexPositioningLogic.CoverUpVertex(interimVertex, layoutAction);

            // TODO: needs test case: when is this necessary?
            //_vertexPositioningLogic.CenterPrimaryParent(positioningEdgePath.Source, layoutAction);

            //TODO: remove from layers
            _layoutGraph.RemovePath(layoutPath);
            _diagramConnectorToLayoutPathMap.Remove(diagramConnector);

            AdjustPathsToLayerSpansRecursive(diagramConnector.Source, layoutAction);
            // TODO: needs test cases
            PositionVerticesOnModifiedPathsRecursive(diagramConnector.Source, layoutAction);

            _vertexPositioningLogic.Compact(layoutAction);
        }

        private void AdjustPathsToLayerSpansRecursive(DiagramNode updateRootNode, ILayoutAction causingAction)
        {
            _diagramGraph.ExecuteOnVerticesRecursive(updateRootNode, EdgeDirection.In,
                i => AdjustPathsToLayerSpans(i, causingAction));
        }

        private void PositionVerticesOnModifiedPathsRecursive(DiagramNode updateRootNode, ILayoutAction causingAction)
        {
            _diagramGraph.ExecuteOnVerticesRecursive(updateRootNode, EdgeDirection.In,
                i => PositionVerticesOnModifiedPaths(i, causingAction));
        }

        private void AdjustPathsToLayerSpans(DiagramNode diagramNode, ILayoutAction causingAction)
        {
            var outConnectors = _diagramGraph.OutEdges(diagramNode);
            foreach (var outConnector in outConnectors)
            {
                var layoutPath = _diagramConnectorToLayoutPathMap.Get(outConnector);
                var diagramConnectorRankSpan = _diagramNodeRankProvider.GetRankSpan(outConnector);

                var pathLengthDifference = diagramConnectorRankSpan - layoutPath.Length;

                if (pathLengthDifference > 0)
                    SplitEdge(layoutPath, 0, pathLengthDifference, causingAction);
                else if (pathLengthDifference < 0)
                    MergeEdgeWithNext(layoutPath, 0, -pathLengthDifference, causingAction);
            }
        }

        private void PositionVerticesOnModifiedPaths(DiagramNode diagramNode, ILayoutAction causingAction)
        {
            var outConnectors = _diagramGraph.OutEdges(diagramNode);
            foreach (var outConnector in outConnectors)
            {
                var layoutPath = _diagramConnectorToLayoutPathMap.Get(outConnector);
                foreach (var edge in layoutPath.Reverse())
                {
                    _vertexPositioningLogic.PositionVertex(edge.Source, causingAction, edge.Target);
                }
            }
        }

        private void SplitEdge(LayoutPath path, int atIndex, int times, ILayoutAction causingAction)
        {
            for (var i = 0; i < times; i++)
                SplitEdge(path, atIndex, causingAction);
        }

        private void SplitEdge(LayoutPath path, int atIndex, ILayoutAction causingAction)
        {
            var edgeToSplit = path[atIndex];
            var interimVertex = new DummyLayoutVertex(true);
            var newEdge1 = new LayoutEdge(edgeToSplit.Source, interimVertex, edgeToSplit.DiagramConnector);
            var newEdge2 = new LayoutEdge(interimVertex, edgeToSplit.Target, edgeToSplit.DiagramConnector);

            path.Substitute(atIndex, 1, newEdge1, newEdge2);

            // TODO: remove from layers
            _layoutGraph.RemoveEdge(edgeToSplit);
            _layoutGraph.AddVertex(interimVertex);
            _layoutGraph.AddEdge(newEdge1);
            _layoutGraph.AddEdge(newEdge2);
            // TODO: add to layers

            RaiseVertexLayoutAction("DummyVertexCreated", interimVertex, causingAction);
        }

        private void MergeEdgeWithNext(LayoutPath path, int atIndex, int times, ILayoutAction causingAction)
        {
            for (var i = 0; i < times; i++)
                MergeEdgeWithNext(path, atIndex, causingAction);
        }

        private void MergeEdgeWithNext(LayoutPath path, int atIndex, ILayoutAction causingAction)
        {
            var firstEdge = path[atIndex];
            var nextEdge = path[atIndex + 1];
            var vertexToRemove = firstEdge.Target as DummyLayoutVertex;
            var mergedEdge = new LayoutEdge(firstEdge.Source, nextEdge.Target, firstEdge.DiagramConnector);

            if (vertexToRemove == null)
                throw new Exception("FirstEdge.Target is null or not dummy!");

            var layoutAction = RaiseVertexLayoutAction("DummyVertexRemoved", vertexToRemove, causingAction);
            _vertexPositioningLogic.CoverUpVertex(vertexToRemove, layoutAction);

            path.Substitute(atIndex, 2, mergedEdge);

            // TODO: remove from layers
            _layoutGraph.RemoveEdge(firstEdge);
            _layoutGraph.RemoveEdge(nextEdge);
            _layoutGraph.RemoveVertex(vertexToRemove);
            _layoutGraph.AddEdge(mergedEdge);
            // TODO: add to layers
        }

        private void OnLayoutActionExecuted(object sender, ILayoutAction layoutAction)
        {
            RaiseLayoutAction(sender, layoutAction);

            var vertexMoveAction = layoutAction as IMoveVertexAction;
            if (vertexMoveAction == null)
                return;

            foreach (var edge in _layoutGraph.GetAllEdges(vertexMoveAction.Vertex))
            {
                var path = _diagramConnectorToLayoutPathMap.Get(edge.DiagramConnector);
                ReroutePath(path, layoutAction);
            }
        }

        private void ReroutePath(LayoutPath path, ILayoutAction causingAction)
        {
            if (path.IsFloating)
                return;

            var oldRoute = _layoutPathToPreviousRouteMap.Get(path);
            var newRoute = path.GetRoute();
            if (oldRoute == newRoute)
                return;

            _layoutPathToPreviousRouteMap.Set(path, newRoute);
            RaisePathLayoutAction("Reroute", path, oldRoute, newRoute, causingAction);
        }
    }
}
