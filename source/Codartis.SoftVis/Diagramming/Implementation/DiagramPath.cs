using System.Collections.Generic;
using Codartis.SoftVis.Graphs;

namespace Codartis.SoftVis.Diagramming.Implementation
{
    /// <summary>
    /// A path created from DiagramConnectors.
    /// </summary>
    public class DiagramPath : Path<IDiagramNode, IDiagramConnector>
    {
        public DiagramPath(IEnumerable<IDiagramConnector> connectors) 
            : base(connectors)
        {
        }
    }
}
