namespace Codartis.SoftVis.VisualStudioIntegration.App.ToggleCommands
{
    /// <summary>
    /// Show or hides all node descriptions. Works like a toggle button.
    /// </summary>
    internal class ShowHideNodeDescriptionsToggleCommand : ToggleCommandBase
    {
        public ShowHideNodeDescriptionsToggleCommand(IAppServices appServices)
            : base(appServices, initialIsChecked: AppDefaults.NodeDescriptionsVisibleByDefault)
        {
            UiService.ExpandAllNodes();
        }

        protected override void OnChecked()
        {
            UiService.ShowDiagramWindow();
            UiService.ExpandAllNodes();
            UiService.KeepDiagramCentered();
        }

        protected override void OnUnchecked()
        {
            UiService.ShowDiagramWindow();
            UiService.CollapseAllNodes();
            UiService.KeepDiagramCentered();
        }
    }
}
