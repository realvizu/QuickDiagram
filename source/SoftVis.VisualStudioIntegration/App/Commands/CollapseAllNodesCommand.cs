namespace Codartis.SoftVis.VisualStudioIntegration.App.Commands
{
    /// <summary>
    /// Collapses all diagram nodes (hide type descriptions).
    /// </summary>
    internal sealed class CollapseAllNodesCommand : SyncCommandWithoutParameterBase
    {
        public CollapseAllNodesCommand(IAppServices appServices)
            : base(appServices)
        { }

        public override void Execute()
        {
            UiService.ShowDiagramWindow();
            UiService.CollapseAllNodes();
            UiService.KeepDiagramCentered();
        }
    }
}
