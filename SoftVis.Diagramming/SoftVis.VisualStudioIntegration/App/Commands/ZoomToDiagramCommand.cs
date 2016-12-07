namespace Codartis.SoftVis.VisualStudioIntegration.App.Commands
{
    /// <summary>
    /// Positions the viewport so that the whole diagram is visible.
    /// </summary>
    internal sealed class ZoomToDiagramCommand : SyncCommandBase
    {
        public ZoomToDiagramCommand(IAppServices appServices)
            : base(appServices)
        { }

        public override void Execute()
        {
            UiServices.ShowDiagramWindow();
            UiServices.FitDiagramToView();
        }
    }
}
