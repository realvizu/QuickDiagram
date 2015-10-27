using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Graphs;
using QuickGraph;

namespace Codartis.SoftVis.Diagramming.Layout.Incremental
{
    /// <summary>
    /// A graph that assigns a layer index to all vertices so for all edges Source.LayerIndex > Target.LayerIndex.
    /// That is, all edges point "upwards".
    /// TODO: Also ensures that the graph is acyclic by reversing edges that cause a cycle.
    /// </summary>
    internal class LayeringGraph : BidirectionalGraph<LayeringVertex, LayeringEdge>
    {
        private readonly PositioningGraph _positioningGraph;
        private readonly Dictionary<LayeringVertex, DiagramNodePositioningVertex> _layeringVertexToPositioningVertexMap;
        private readonly Dictionary<LayeringEdge, PositioningEdgePath> _layeringEdgeToPositioningEdgePathMap;

        public LayeringGraph()
            : base(allowParallelEdges: false)
        {
            _positioningGraph = new PositioningGraph();
            _layeringVertexToPositioningVertexMap = new Dictionary<LayeringVertex, DiagramNodePositioningVertex>();
            _layeringEdgeToPositioningEdgePathMap = new Dictionary<LayeringEdge, PositioningEdgePath>();
        }

        public override bool AddVertex(LayeringVertex layeringVertex)
        {
            var isAdded = base.AddVertex(layeringVertex);
            if (isAdded)
            {
                var positioningVertex = new DiagramNodePositioningVertex(layeringVertex.DiagramNode, layeringVertex.LayerIndex, true);
                _positioningGraph.AddVertex(positioningVertex);
                _layeringVertexToPositioningVertexMap.Add(layeringVertex, positioningVertex);
            }
            return isAdded;
        }

        public override bool RemoveVertex(LayeringVertex layeringVertex)
        {
            var isRemoved = base.RemoveVertex(layeringVertex);
            if (isRemoved)
            {
                var positioningVertex = _layeringVertexToPositioningVertexMap[layeringVertex];
                _positioningGraph.RemoveVertex(positioningVertex);
                _layeringVertexToPositioningVertexMap.Remove(layeringVertex);
            }
            return isRemoved;
        }

        public override bool AddEdge(LayeringEdge layeringEdge)
        {
            var isAdded = base.AddEdge(layeringEdge);
            if (isAdded)
            {
                var positioningSource = _layeringVertexToPositioningVertexMap[layeringEdge.Source];
                var positioningTarget = _layeringVertexToPositioningVertexMap[layeringEdge.Target];
                var positioningEdge = new PositioningEdge(positioningSource, positioningTarget);
                var positioningEdgePath = new PositioningEdgePath(positioningEdge);
                _positioningGraph.AddPath(positioningEdgePath);

                _layeringEdgeToPositioningEdgePathMap.Add(layeringEdge, positioningEdgePath);

                UpdateLayerIndexesRecursive(layeringEdge.Source);
                UpdatePositioningEdgePathsRecursive(layeringEdge.Source);
            }
            return isAdded;
        }

        public override bool RemoveEdge(LayeringEdge layeringEdge)
        {
            var isRemoved = base.RemoveEdge(layeringEdge);
            if (isRemoved)
            {
                var positioningEdgePath = _layeringEdgeToPositioningEdgePathMap[layeringEdge];
                _positioningGraph.RemovePath(positioningEdgePath);
                _layeringEdgeToPositioningEdgePathMap.Remove(layeringEdge);

                UpdateLayerIndexesRecursive(layeringEdge.Source);
                UpdatePositioningEdgePathsRecursive(layeringEdge.Source);
            }
            return isRemoved;
        }

        private void UpdateLayerIndexesRecursive(LayeringVertex updateRootLayeringVertex)
        {
            this.ExecuteOnVerticesRecursive(updateRootLayeringVertex, EdgeDirection.In, UpdateLayerIndex);
        }

        private void UpdateLayerIndex(LayeringVertex layeringVertex)
        {
            layeringVertex.LayerIndex = CalculateLayerIndex(layeringVertex);
            var positioningVertex = _layeringVertexToPositioningVertexMap[layeringVertex];
            positioningVertex.LayerIndex = layeringVertex.LayerIndex;
        }

        private int CalculateLayerIndex(LayeringVertex layeringVertex)
        {
            return OutEdges(layeringVertex).Select(i => i.Target.LayerIndex).DefaultIfEmpty(-1).Max() + 1;
        }

        private void UpdatePositioningEdgePathsRecursive(LayeringVertex updateRootLayeringVertex)
        {
            this.ExecuteOnVerticesRecursive(updateRootLayeringVertex, EdgeDirection.In, UpdatePositioningEdgePaths);
        }

        private void UpdatePositioningEdgePaths(LayeringVertex layeringVertex)
        {
            var layeringOutEdges = OutEdges(layeringVertex);
            foreach (var layeringOutEdge in layeringOutEdges)
            {
                var positioningEdgePath = _layeringEdgeToPositioningEdgePathMap[layeringOutEdge];
                var pathLengthDifference = layeringOutEdge.LayerSpan - positioningEdgePath.Length;

                if (pathLengthDifference > 0)
                    positioningEdgePath.SplitEdge(0, pathLengthDifference);
                else if (pathLengthDifference < 0)
                    positioningEdgePath.MergeEdgeWithNext(0, -pathLengthDifference);
            }
        }
    }
}
