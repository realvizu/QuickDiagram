using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Graphs;

namespace Codartis.SoftVis.Diagramming.Layout.Incremental
{
    internal class LayoutPath : Path<LayoutVertex, LayoutEdge>
    {
        public LayoutPath(LayoutEdge layoutEdge)
            : base(layoutEdge)
        {
        }

        public LayoutPath(IEnumerable<LayoutEdge> layoutEdges)
            : base(layoutEdges)
        {
        }

        public IEnumerable<LayoutVertex> GetInterimVertices()
        {
            return this.Skip(1).Select(i => i.Source);
        }
    }
}
