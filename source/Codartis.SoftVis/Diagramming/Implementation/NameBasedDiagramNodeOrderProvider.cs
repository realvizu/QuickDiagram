using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Diagramming.Definition;

namespace Codartis.SoftVis.Diagramming.Implementation
{
    public sealed class NameBasedDiagramNodeOrderProvider : IDiagramNodeOrderProvider
    {
        public IEnumerable<IDiagramNode> OrderNodes(IEnumerable<IDiagramNode> diagramNodes)
        {
            return diagramNodes
                .OrderBy(i => i.Name);
        }
    }
}