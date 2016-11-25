namespace Codartis.SoftVis.VisualStudioIntegration.App.Commands
{
    /// <summary>
    /// Adds the current symbol (the one at the caret) to the diagram.
    /// Shows the diagram if it was not visible.
    /// </summary>
    internal sealed class AddToDiagramCommand : ParameterlessCommandBase
    {
        public AddToDiagramCommand(IAppServices appServices)
            :base(appServices)
        {
        }

        public override async void Execute()
        {
            var modelEntity = await ModelServices.AddCurrentSymbolAsync();
            if (modelEntity == null)
                return;

            DiagramServices.ShowModelEntity(modelEntity);
            UiServices.ShowDiagramWindow();
            UiServices.FitDiagramToView();
        }
    }
}
