using System.Collections.Generic;
using System.Linq;
using QuickGraph;

namespace Codartis.SoftVis.Diagramming.Graph.Layout.VertexPlacement.Incremental
{
    internal sealed class LayoutGraph : BidirectionalGraph<LayoutVertex, LayoutEdge>
    {
        public IEnumerable<LayoutEdge> GetAllEdges(LayoutVertex layoutVertex)
        {
            return InEdges(layoutVertex).Union(OutEdges(layoutVertex));
        }
    }
}
