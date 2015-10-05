using System.Collections.Generic;
using Codartis.SoftVis.Graphs;

namespace Codartis.SoftVis.Diagramming
{
    internal class DiagramPath : Path<DiagramNode, DiagramConnector>
    {
        public DiagramPath(IEnumerable<DiagramConnector> edges) 
            : base(edges)
        {
        }
    }
}
