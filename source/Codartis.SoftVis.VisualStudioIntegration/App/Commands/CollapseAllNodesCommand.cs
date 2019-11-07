using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Codartis.SoftVis.VisualStudioIntegration.App.Commands
{
    /// <summary>
    /// Collapses all diagram nodes (hide type descriptions).
    /// </summary>
    [UsedImplicitly]
    internal sealed class CollapseAllNodesCommand : CommandBase
    {
        public CollapseAllNodesCommand(IAppServices appServices)
            : base(appServices)
        { }

        public override async Task ExecuteAsync()
        {
            await HostUiService.ShowDiagramWindowAsync();
            DiagramWindowService.CollapseAllNodes();
            DiagramWindowService.KeepDiagramCentered();
        }
    }
}
