using System;
using Microsoft.CodeAnalysis;

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
            var namedTypeSymbol = await HostWorkspaceServices.GetCurrentSymbol() as INamedTypeSymbol;
            if (namedTypeSymbol == null)
                return;

            var modelEntity = ModelServices.GetOrAddRoslynSymbol(namedTypeSymbol);
            if (modelEntity == null)
                return;

            DiagramServices.ShowModelEntity(modelEntity);
            HostUiServices.DiagramHostWindow.Show();
            UiServices.FitDiagramToView();
        }
    }
}
