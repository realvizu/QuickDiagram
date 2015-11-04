using System;
using System.Collections.Generic;

namespace Codartis.SoftVis.Diagramming.Layout.Incremental
{
    /// <summary>
    /// Comparer used to sort sibling vertices in layers.
    /// </summary>
    internal class VerticesInLayerComparer : IComparer<PositioningVertexBase>
    {
        private readonly IReadOnlyPositioningGraph _positioningGraph;

        public VerticesInLayerComparer(IReadOnlyPositioningGraph positioningGraph)
        {
            _positioningGraph = positioningGraph;
        }

        public int Compare(PositioningVertexBase vertex1, PositioningVertexBase vertex2)
        {
            if (vertex1 == null) throw new ArgumentNullException(nameof(vertex1));
            if (vertex2 == null) throw new ArgumentNullException(nameof(vertex2));

            return string.Compare(GetNameForComparison(vertex1), GetNameForComparison(vertex2),
                StringComparison.InvariantCultureIgnoreCase);
        }

        private string GetNameForComparison(PositioningVertexBase vertex)
        {
            var dummyPositioningVertex = vertex as DummyPositioningVertex;
            if (dummyPositioningVertex != null)
            {
                var inEdge = _positioningGraph.GetInEdge(dummyPositioningVertex);
                if (inEdge == null)
                    throw new InvalidOperationException("Dummy vertex with no in-edge cannot be compared.");
                return GetNameForComparison(inEdge.Source);
            }
            return vertex.Name;
        }
    }
}
