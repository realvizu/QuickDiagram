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
        IModelServices ModelServices { get; }
        IDiagramServices DiagramServices { get; }
        IUiServices UiServices { get; }
    }
}
