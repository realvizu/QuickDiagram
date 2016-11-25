using System.Threading.Tasks;

namespace Codartis.SoftVis.VisualStudioIntegration.App.Commands
{
    /// <summary>
    /// Adds the current symbol (the one at the caret) to the diagram.
    /// Shows the diagram if it was not visible.
    /// </summary>
    internal sealed class AddToDiagramCommand : AsyncCommandBase
    {
        public AddToDiagramCommand(IAppServices appServices)
            :base(appServices)
        {
        }

        public override async Task ExecuteAsync()
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
