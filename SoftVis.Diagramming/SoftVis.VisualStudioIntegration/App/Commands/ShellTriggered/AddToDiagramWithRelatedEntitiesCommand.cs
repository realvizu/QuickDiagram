using System;
using Microsoft.CodeAnalysis;

namespace Codartis.SoftVis.VisualStudioIntegration.App.Commands.ShellTriggered
{
    /// <summary>
    /// Adds the current symbol (the one at the caret) and its related entities to the diagram.
    /// Shows the diagram if it was not visible.
    /// </summary>
    internal sealed class AddToDiagramWithRelatedEntitiesCommand : ShellTriggeredCommandBase
    {
        public AddToDiagramWithRelatedEntitiesCommand(IAppServices appServices)
            :base(VsctConstants.SoftVisCommandSetGuid, VsctConstants.AddToDiagramWithRelatedEntitiesCommand, appServices)
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

            DiagramServices.ShowModelEntityWithRelatedEntities(modelEntity);
            HostUiServices.DiagramHostWindow.Show();
            UiServices.FitDiagramToView();
        }
    }
}
