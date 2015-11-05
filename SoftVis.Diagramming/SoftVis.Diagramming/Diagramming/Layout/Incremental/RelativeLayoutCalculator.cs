using System;
using Codartis.SoftVis.Diagramming.Layout.ActionTracking;
using Codartis.SoftVis.Graphs;

namespace Codartis.SoftVis.Diagramming.Layout.Incremental
{
    /// <summary>
    /// Calculates the arrangement of layout vertices relative to each other 
    /// and updates LayoutGraph and LayoutVertexLayers accordingly.
    /// </summary>
    internal class RelativeLayoutCalculator : IncrementalLayoutActionEventSource
    {
        private readonly IReadOnlyDiagramGraph _diagramGraph;
        private readonly LayoutGraph _layoutGraph;
        private readonly LayoutVertexLayers _layers;

        public RelativeLayoutCalculator(IReadOnlyDiagramGraph diagramGraph,
            LayoutGraph layoutGraph, LayoutVertexLayers layers)
        {
            _diagramGraph = diagramGraph;
            _layoutGraph = layoutGraph;
            _layers = layers;
        }

        public void Clear()
        {
        }

        public void Add(DiagramNodeLayoutVertex diagramNodeVertex)
        {
            RaiseVertexLayoutAction("Relative.AddVertex", diagramNodeVertex);

            AddVertex(diagramNodeVertex);
        }

        public void Remove(DiagramNodeLayoutVertex diagramNodeVertex)
        {
            RaiseVertexLayoutAction("Relative.RemoveVertex", diagramNodeVertex);

            RemoveVertex(diagramNodeVertex);
        }

        public void Add(LayoutPath layoutPath)
        {
            var layoutAction = RaisePathLayoutAction("Relative.AddPath", layoutPath);

            AddPath(layoutPath);

            //AdjustPathsToLayerSpansRecursive(layoutPath.Source, layoutAction);
        }

        public void Remove(LayoutPath layoutPath)
        {
            RaisePathLayoutAction("Relative.RemovePath", layoutPath);

            RemovePath(layoutPath);
        }

        private void AddVertex(LayoutVertexBase vertex)
        {
            _layoutGraph.AddVertex(vertex);
            _layers.AddVertex(vertex);
        }

        private void RemoveVertex(LayoutVertexBase vertex)
        {
            _layers.RemoveVertex(vertex);
            _layoutGraph.RemoveVertex(vertex);
        }

        private void AddPath(LayoutPath path)
        {
            foreach (var interimVertex in path.InterimVertices)
                AddVertex(interimVertex);

            foreach (var edge in path)
                AddEdge(edge);
        }

        private void RemovePath(LayoutPath path)
        {
            foreach (var edge in path)
                RemoveEdge(edge);

            foreach (var interimVertex in path.InterimVertices)
                RemoveVertex(interimVertex);
        }

        private void AddEdge(LayoutEdge edge)
        {
            _layoutGraph.AddEdge(edge);
            _layers.AddEdge(edge);
        }

        private void RemoveEdge(LayoutEdge edge)
        {
            _layers.RemoveEdge(edge);
            _layoutGraph.RemoveEdge(edge);
        }
        
        //private void AdjustPathsToLayerSpansRecursive(DiagramNode updateRootNode, ILayoutAction causingAction)
        //{
        //    _diagramGraph.ExecuteOnVerticesRecursive(updateRootNode, EdgeDirection.In,
        //        i => AdjustPathsToLayerSpans(i, causingAction));
        //}

        //private void AdjustPathsToLayerSpans(DiagramNode diagramNode, ILayoutAction causingAction)
        //{
        //    var outConnectors = _diagramGraph.OutEdges(diagramNode);
        //    foreach (var outConnector in outConnectors)
        //    {
        //        var layoutPath = _diagramConnectorToLayoutPathMap.Get(outConnector);
        //        var diagramConnectorRankSpan = _layers.GetLayerIndex(layoutPath.Source) 
        //            - _layers.GetLayerIndex(layoutPath.Target);

        //        var pathLengthDifference = diagramConnectorRankSpan - layoutPath.Length;

        //        if (pathLengthDifference > 0)
        //            SplitEdge(layoutPath, 0, pathLengthDifference, causingAction);
        //        else if (pathLengthDifference < 0)
        //            MergeEdgeWithNext(layoutPath, 0, -pathLengthDifference, causingAction);
        //    }
        //}

        //private void SplitEdge(LayoutPath path, int atIndex, int times, ILayoutAction causingAction)
        //{
        //    for (var i = 0; i < times; i++)
        //        SplitEdge(path, atIndex, causingAction);
        //}

        //private void SplitEdge(LayoutPath path, int atIndex, ILayoutAction causingAction)
        //{
        //    var edgeToSplit = path[atIndex];
        //    var interimVertex = new DummyLayoutVertex(true);
        //    var newEdge1 = new LayoutEdge(edgeToSplit.Source, interimVertex, edgeToSplit.DiagramConnector);
        //    var newEdge2 = new LayoutEdge(interimVertex, edgeToSplit.Target, edgeToSplit.DiagramConnector);

        //    path.Substitute(atIndex, 1, newEdge1, newEdge2);

        //    RemoveEdge(edgeToSplit);
        //    AddVertex(interimVertex);
        //    AddEdge(newEdge1);
        //    AddEdge(newEdge2);

        //    RaiseVertexLayoutAction("DummyVertexCreated", interimVertex, causingAction);
        //}

        //private void MergeEdgeWithNext(LayoutPath path, int atIndex, int times, ILayoutAction causingAction)
        //{
        //    for (var i = 0; i < times; i++)
        //        MergeEdgeWithNext(path, atIndex, causingAction);
        //}

        //private void MergeEdgeWithNext(LayoutPath path, int atIndex, ILayoutAction causingAction)
        //{
        //    var firstEdge = path[atIndex];
        //    var nextEdge = path[atIndex + 1];
        //    var vertexToRemove = firstEdge.Target as DummyLayoutVertex;
        //    var mergedEdge = new LayoutEdge(firstEdge.Source, nextEdge.Target, firstEdge.DiagramConnector);

        //    if (vertexToRemove == null)
        //        throw new Exception("FirstEdge.Target is null or not dummy!");

        //    RaiseVertexLayoutAction("DummyVertexRemoved", vertexToRemove, causingAction);

        //    path.Substitute(atIndex, 2, mergedEdge);

        //    RemoveEdge(firstEdge);
        //    RemoveEdge(nextEdge);
        //    RemoveVertex(vertexToRemove);
        //    AddEdge(mergedEdge);
        //}
    }
}