using Codartis.SoftVis.VisualStudioIntegration.Diagramming;
using Codartis.SoftVis.VisualStudioIntegration.Modeling;
using Codartis.SoftVis.VisualStudioIntegration.WorkspaceContext;

namespace Codartis.SoftVis.VisualStudioIntegration.Commands
{
    /// <summary>
    /// These are the operations exposed to the commands by the VS package.
    /// </summary>
    internal interface IPackageServices
    {
        IWorkspaceServices GetWorkspaceServices();
        IModelServices GetModelServices();
        IDiagramServices GetDiagramServices();
    }
}
