namespace Codartis.SoftVis.VisualStudioIntegration.App.Commands
{
    /// <summary>
    /// Clears the model (and subsequently the diagram).
    /// </summary>
    internal sealed class ClearModelCommand : SyncCommandBase
    {
        public ClearModelCommand(IAppServices appServices)
            : base(appServices)
        { }

        public override void Execute()
        {
            UiService.ShowDiagramWindow();
            ModelService.ClearModel();
        }
    }
}
