using System.Threading.Tasks;

namespace Codartis.SoftVis.VisualStudioIntegration.App.ToggleCommands
{
    /// <summary>
    /// Show or hides all node descriptions. Works like a toggle button.
    /// </summary>
    internal class ShowHideNodeDescriptionsToggleCommand : ToggleCommandBase
    {
        public ShowHideNodeDescriptionsToggleCommand(IAppServices appServices)
            : base(appServices, initialIsChecked: true)
        {
            UiServices.ExpandAllNodes();
        }

        protected override async Task OnCheckedAsync()
        {
            await HostUiServices.ShowDiagramWindowAsync();
            UiServices.ExpandAllNodes();
            UiServices.KeepDiagramCentered();
        }

        protected override async Task OnUncheckedAsync()
        {
            await HostUiServices.ShowDiagramWindowAsync();
            UiServices.CollapseAllNodes();
            UiServices.KeepDiagramCentered();
        }
    }
}
