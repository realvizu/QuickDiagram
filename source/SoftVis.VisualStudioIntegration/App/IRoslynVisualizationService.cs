using System.Threading;
using System.Threading.Tasks;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.Service;
using Codartis.SoftVis.Util;
using Codartis.SoftVis.VisualStudioIntegration.Modeling;

namespace Codartis.SoftVis.VisualStudioIntegration.App
{
    /// <summary>
    /// Extends the visualization service with roslyn-specific operations.
    /// </summary>
    internal interface IRoslynVisualizationService : IVisualizationService
    {
        Task<bool> IsCurrentSymbolAvailableAsync();
        Task<IRoslynModelNode> AddCurrentSymbolAsync();

        void ExtendModelWithRelatedNodes(IRoslynModelNode modelNode, DirectedModelRelationshipType? directedModelRelationshipType = null,
            CancellationToken cancellationToken = default(CancellationToken), IIncrementalProgress progress = null, bool recursive = false);

        void ShowModelNodeWithHierarchy(DiagramId diagramId, IRoslynModelNode modelNode,
            CancellationToken cancellationToken, IIncrementalProgress progress);

        bool HasSource(IRoslynModelNode modelNode);
        void ShowSource(IRoslynModelNode modelNode);
    }
}
