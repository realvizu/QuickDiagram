using Codartis.SoftVis.VisualStudioIntegration.Diagramming;
using Codartis.SoftVis.VisualStudioIntegration.Modeling;
using Codartis.SoftVis.VisualStudioIntegration.UI;

namespace Codartis.SoftVis.VisualStudioIntegration.App
{
    /// <summary>
    /// These are the application services that the application commands can use.
    /// </summary>
    public interface IAppServices
    {
        IHostUiServices GetHostUiServices();
        IHostWorkspaceServices GetHostWorkspaceServices();
        IModelServices GetModelServices();
        IDiagramServices GetDiagramServices();
        IUiServices GetUiServices();
    }
}
