using System.Threading.Tasks;

namespace Codartis.SoftVis.VisualStudioIntegration.App.Commands
{
    /// <summary>
    /// Expands all diagram nodes (show type descriptions).
    /// </summary>
    internal sealed class ExpandAllNodesCommand : AsyncCommandBase
    {
        public ExpandAllNodesCommand(IAppServices appServices)
            : base(appServices)
        {
        }

        public override async Task ExecuteAsync()
        {
            await HostUiServices.ShowDiagramWindowAsync();
            UiServices.ExpandAllNodes();
            UiServices.KeepDiagramCentered();
        }
    }
}