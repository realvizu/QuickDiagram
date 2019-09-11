using System;
using System.Collections.Generic;

namespace Codartis.SoftVis.Diagramming.Implementation.Layout.Sugiyama.Relative.Logic
{
    /// <summary>
    /// Comparer used to sort sibling vertices.
    /// </summary>
    internal class SiblingsComparer : IComparer<LayoutVertexBase>
    {
        private readonly IReadOnlyQuasiProperLayoutGraph _properLayeredLayoutGraph;

        public SiblingsComparer(IReadOnlyQuasiProperLayoutGraph properLayeredLayoutGraph)
        {
            _properLayeredLayoutGraph = properLayeredLayoutGraph;
        }

        public int Compare(LayoutVertexBase vertex1, LayoutVertexBase vertex2)
        {
            if (vertex1 == null) throw new ArgumentNullException(nameof(vertex1));
            if (vertex2 == null) throw new ArgumentNullException(nameof(vertex2));

            return string.Compare(GetNameForComparison(vertex1), GetNameForComparison(vertex2),
                StringComparison.InvariantCultureIgnoreCase);
        }

        private string GetNameForComparison(LayoutVertexBase vertex)
        {
            if (vertex is DummyLayoutVertex dummyVertex)
            {
                var inEdge = _properLayeredLayoutGraph.GetInEdge(dummyVertex);
                if (inEdge == null)
                    throw new InvalidOperationException("Dummy vertex with no in-edge cannot be compared.");
                return GetNameForComparison(inEdge.Source);
            }
            return vertex.Name;
        }
    }
}
