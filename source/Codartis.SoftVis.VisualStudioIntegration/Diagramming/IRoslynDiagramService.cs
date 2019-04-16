using System.Collections.Generic;
using System.Threading;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.VisualStudioIntegration.Modeling;
using Codartis.Util;

namespace Codartis.SoftVis.VisualStudioIntegration.Diagramming
{
    /// <summary>
    /// Roslyn-specific extensions to diagram operations.
    /// </summary>
    internal interface IRoslynDiagramService : IDiagramService
    {
        IEnumerable<IDiagramNode> ShowModelNodeWithHierarchy(IRoslynModelNode modelNode, 
            CancellationToken cancellationToken, IIncrementalProgress progress);
    }
}