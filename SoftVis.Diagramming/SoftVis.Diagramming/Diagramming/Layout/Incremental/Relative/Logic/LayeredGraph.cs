using System;
using Codartis.SoftVis.Common;

namespace Codartis.SoftVis.Diagramming.Layout.Incremental.Relative.Logic
{
    /// <summary>
    /// A graph for layout calculation that also tracks the layering of vertices.
    /// Contains vertices for diagram nodes and layout paths (layout edge sequences) for diagram connectors.
    /// </summary>
    /// <remarks>
    /// Invariant:
    /// <para>For all vertices and all of their parents: LayerIndex(vertex) > LayerIndex(vertex.Parent)</para>
    /// </remarks>
    internal sealed class LayeredGraph : LayoutGraphBase<DiagramNodeLayoutVertex, LayoutPath>,
        IReadOnlyLayeredGraph
    {
        private readonly Map<DiagramNodeLayoutVertex, int> _vertexToLayerIndexMap;

        public LayeredGraph()
        {
            _vertexToLayerIndexMap = new Map<DiagramNodeLayoutVertex, int>();
        }

        public override bool AddVertex(DiagramNodeLayoutVertex vertex)
        {
            var isAdded = base.AddVertex(vertex);
            if (isAdded)
                _vertexToLayerIndexMap.Set(vertex, 0);

            CheckInvariant();
            return isAdded;
        }

        public override bool RemoveVertex(DiagramNodeLayoutVertex vertex)
        {
            var isRemoved = base.RemoveVertex(vertex);
            if (isRemoved)
                _vertexToLayerIndexMap.Remove(vertex);

            CheckInvariant();
            return isRemoved;
        }

        public override bool AddEdge(LayoutPath edge)
        {
            var isAdded = base.AddEdge(edge);
            if (isAdded)
                ExecuteOnDescendantVertices(edge.PathSource, UpdateLayerIndex);

            CheckInvariant();
            return isAdded;
        }

        public override bool RemoveEdge(LayoutPath edge)
        {
            var isRemoved = base.RemoveEdge(edge);

            CheckInvariant();
            return isRemoved;
        }

        public int GetLayerIndex(DiagramNodeLayoutVertex vertex)
        {
            if (!_vertexToLayerIndexMap.ContainsKey(vertex))
                throw new ArgumentException($"Vertex {vertex} is not in the layered graph.");

            return _vertexToLayerIndexMap.Get(vertex);
        }

        private void UpdateLayerIndex(DiagramNodeLayoutVertex vertex)
        {
            var newLayerIndex = CalculateLayerIndex(vertex);
            _vertexToLayerIndexMap.Set(vertex, newLayerIndex);
        }

        private int CalculateLayerIndex(DiagramNodeLayoutVertex vertex)
        {
            var currentLayerIndex = _vertexToLayerIndexMap.Get(vertex);
            var minimumLayerIndex = GetRank(vertex);
            return Math.Max(currentLayerIndex, minimumLayerIndex);
        }

        private void CheckInvariant()
        {
            foreach (var vertex in Vertices)
            {
                var layerIndex = _vertexToLayerIndexMap.Get(vertex);
                foreach (var parentVertex in GetParents(vertex))
                {
                    var parentLayerIndex = _vertexToLayerIndexMap.Get(parentVertex);
                    if (layerIndex <= parentLayerIndex)
                        throw new Exception($"Vertex {vertex} at layer {layerIndex} should be higher then its parent {parentVertex} at {parentLayerIndex}.");
                }
            }
        }

    }
}
