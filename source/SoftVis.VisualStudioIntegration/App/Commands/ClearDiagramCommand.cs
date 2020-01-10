using System.Threading.Tasks;

namespace Codartis.SoftVis.VisualStudioIntegration.App.Commands
{
    /// <summary>
    /// Clears the diagram.
    /// </summary>
    internal sealed class ClearDiagramCommand : AsyncCommandBase
    {
        public ClearDiagramCommand(IAppServices appServices)
            : base(appServices)
        { }

        public override async Task ExecuteAsync()
        {
            await HostUiServices.ShowDiagramWindowAsync();
            DiagramServices.Clear();
        }
    }
}
