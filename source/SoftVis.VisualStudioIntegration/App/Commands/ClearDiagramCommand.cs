namespace Codartis.SoftVis.VisualStudioIntegration.App.Commands
{
    /// <summary>
    /// Clears the diagram.
    /// </summary>
    internal sealed class ClearDiagramCommand : SyncCommandWithoutParameterBase
    {
        public ClearDiagramCommand(IAppServices appServices)
            : base(appServices)
        { }

        public override void Execute()
        {
            UiService.ShowDiagramWindow();
            DiagramServices.ClearDiagram();
        }
    }
}
