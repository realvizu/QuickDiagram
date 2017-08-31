namespace Codartis.SoftVis.VisualStudioIntegration.App.Commands
{
    /// <summary>
    /// Expands all diagram nodes (show type descriptions).
    /// </summary>
    internal sealed class ExpandAllNodesCommand : SyncCommandBase
    {
        public ExpandAllNodesCommand(IAppServices appServices)
            : base(appServices)
        { }

        public override void Execute()
        {
            UiService.ShowDiagramWindow();
            UiService.ExpandAllNodes();
            UiService.KeepDiagramCentered();
        }
    }
}
