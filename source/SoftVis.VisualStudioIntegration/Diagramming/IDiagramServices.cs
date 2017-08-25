using System.Collections.Generic;
using System.Threading;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Util;
using Codartis.SoftVis.VisualStudioIntegration.Modeling;

namespace Codartis.SoftVis.VisualStudioIntegration.Diagramming
{
    /// <summary>
    /// Defines diagram operations for the application commands package.
    /// </summary>
    internal interface IDiagramServices : IDiagramStore
    {
        IDiagramNode ShowModelNode(IRoslynModelNode modelNode);
        IReadOnlyList<IDiagramNode> ShowModelNodes(IEnumerable<IRoslynModelNode> modelNodes, CancellationToken cancellationToken, IIncrementalProgress progress);
        IReadOnlyList<IDiagramNode> ShowModelNodeWithHierarchy(IRoslynModelNode modelNode, CancellationToken cancellationToken, IIncrementalProgress progress);

        void Clear();
        void UpdateFromSource(CancellationToken cancellationToken, IIncrementalProgress progress);
    }
}
