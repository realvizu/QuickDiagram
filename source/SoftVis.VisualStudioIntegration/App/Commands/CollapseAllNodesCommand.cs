using System.Threading.Tasks;

namespace Codartis.SoftVis.VisualStudioIntegration.App.Commands
{
    /// <summary>
    /// Collapses all diagram nodes (hide type descriptions).
    /// </summary>
    internal sealed class CollapseAllNodesCommand : AsyncCommandBase
    {
        public CollapseAllNodesCommand(IAppServices appServices)
            : base(appServices)
        { }

        public override async Task ExecuteAsync()
        {
            await HostUiServices.ShowDiagramWindowAsync();
            UiServices.CollapseAllNodes();
            UiServices.KeepDiagramCentered();
        }
    }
}
