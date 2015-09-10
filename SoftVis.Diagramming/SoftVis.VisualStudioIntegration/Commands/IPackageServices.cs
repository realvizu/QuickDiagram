using Codartis.SoftVis.VisualStudioIntegration.Presentation;

namespace Codartis.SoftVis.VisualStudioIntegration.Commands
{
    /// <summary>
    /// These are the operations exposed to the commands by the VS package.
    /// </summary>
    internal interface IPackageServices
    {
        IDiagramWindow GetDiagramWindow();
    }
}
