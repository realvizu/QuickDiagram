using System.Threading.Tasks;

namespace Codartis.SoftVis.VisualStudioIntegration.App.Commands
{
    /// <summary>
    /// Positions the viewport so that the whole diagram is visible.
    /// </summary>
    internal sealed class ZoomToDiagramCommand : AsyncCommandBase
    {
        public ZoomToDiagramCommand(IAppServices appServices)
            : base(appServices)
        { }

        public override async Task ExecuteAsync()
        {
            await HostUiServices.ShowDiagramWindowAsync();
            UiServices.ZoomToDiagram();
        }
    }
}
