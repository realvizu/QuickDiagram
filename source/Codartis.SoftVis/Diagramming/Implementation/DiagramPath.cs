using System.Collections.Generic;
using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.Graphs;
using Codartis.SoftVis.Modeling.Definition;

namespace Codartis.SoftVis.Diagramming.Implementation
{
    /// <summary>
    /// A path created from DiagramConnectors.
    /// </summary>
    public class DiagramPath : Path<ModelNodeId, IDiagramConnector>
    {
        public DiagramPath(IEnumerable<IDiagramConnector> connectors) 
            : base(connectors)
        {
        }
    }
}
