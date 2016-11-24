using System;

namespace Codartis.SoftVis.VisualStudioIntegration.App.Commands.ShellTriggered
{
    /// <summary>
    /// Adds the current symbol (the one at the caret) to the diagram.
    /// Shows the diagram if it was not visible.
    /// </summary>
    internal sealed class AddToDiagramCommand : ShellTriggeredCommandBase
    {
        public AddToDiagramCommand(IAppServices appServices)
            :base(VsctConstants.SoftVisCommandSetGuid, VsctConstants.AddToDiagramCommand, appServices)
        {
        }

        public override async void Execute(object sender, EventArgs e)
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
