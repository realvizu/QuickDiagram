namespace Codartis.SoftVis.VisualStudioIntegration.App.ToggleCommands
{
    /// <summary>
    /// Show or hides all node descriptions. Works like a toggle button.
    /// </summary>
    internal class ShowHideNodeDescriptionsToggleCommand : ToggleCommandBase
    {
        public ShowHideNodeDescriptionsToggleCommand(IAppServices appServices) 
            : base(appServices, initialIsChecked: false)
        {
        }

        protected override void OnChecked()
        {
            UiServices.ShowDiagramWindow();
            UiServices.ExpandAllNodes();
            UiServices.KeepDiagramCentered();
        }

        protected override void OnUnchecked()
        {
            UiServices.ShowDiagramWindow();
            UiServices.CollapseAllNodes();
            UiServices.KeepDiagramCentered();
        }
    }
}
