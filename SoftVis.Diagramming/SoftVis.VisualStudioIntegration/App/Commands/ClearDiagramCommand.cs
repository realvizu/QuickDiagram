namespace Codartis.SoftVis.VisualStudioIntegration.App.Commands
{
    /// <summary>
    /// Clears the diagram.
    /// </summary>
    internal sealed class ClearDiagramCommand : ParameterlessCommandBase
    {
        public ClearDiagramCommand(IAppServices appServices)
            : base(appServices)
        { }

        public override void Execute()
        {
            UiServices.ShowDiagramWindow();
            DiagramServices.Clear();
        }
    }
}
