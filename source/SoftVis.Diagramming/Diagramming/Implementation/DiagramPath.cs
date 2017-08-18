using System.Collections.Generic;
using Codartis.SoftVis.Graphs;

namespace Codartis.SoftVis.Diagramming.Implementation
{
    /// <summary>
    /// A path created from DiagramConnectors.
    /// </summary>
    internal class DiagramPath : Path<IDiagramNode, DiagramConnector>
    {
        public DiagramPath(IEnumerable<DiagramConnector> edges) 
            : base(edges)
        {
        }
    }
}
