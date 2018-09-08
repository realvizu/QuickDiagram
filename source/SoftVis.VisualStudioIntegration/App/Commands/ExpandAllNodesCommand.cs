using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Codartis.SoftVis.VisualStudioIntegration.App.Commands
{
    /// <summary>
    /// Expands all diagram nodes (show type descriptions).
    /// </summary>
    [UsedImplicitly]
    internal sealed class ExpandAllNodesCommand : CommandBase
    {
        public ExpandAllNodesCommand(IAppServices appServices)
            : base(appServices)
        { }

        public override async Task ExecuteAsync()
        {
            await UiService.ShowDiagramWindowAsync();
            UiService.ExpandAllNodes();
            UiService.KeepDiagramCentered();
        }
    }
}
