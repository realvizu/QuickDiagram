using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Codartis.SoftVis.VisualStudioIntegration.App.Commands.Toggle
{
    /// <summary>
    /// Show or hides all node descriptions. Works like a toggle button.
    /// </summary>
    [UsedImplicitly]
    internal class ShowHideNodeDescriptionsToggleCommand : ToggleCommandBase
    {
        public ShowHideNodeDescriptionsToggleCommand(IAppServices appServices)
            : base(appServices, initialIsChecked: AppDefaults.NodeDescriptionsVisibleByDefault)
        {
            UiService.ExpandAllNodes();
        }

        protected override async Task OnCheckedAsync()
        {
           await UiService.ShowDiagramWindowAsync();
            UiService.ExpandAllNodes();
            UiService.KeepDiagramCentered();
        }

        protected override async Task OnUncheckedAsync()
        {
            await UiService.ShowDiagramWindowAsync();
            UiService.CollapseAllNodes();
            UiService.KeepDiagramCentered();
        }
    }
}
