using System.Collections.Generic;
using Codartis.SoftVis.Graphs;

namespace Codartis.SoftVis.Diagramming.Graph
{
    /// <summary>
    /// A path created from DiagramConnectors.
    /// </summary>
    internal class DiagramPath : Path<DiagramNode, DiagramConnector>
    {
        public DiagramPath(IEnumerable<DiagramConnector> edges) 
            : base(edges)
        {
        }
    }
}
