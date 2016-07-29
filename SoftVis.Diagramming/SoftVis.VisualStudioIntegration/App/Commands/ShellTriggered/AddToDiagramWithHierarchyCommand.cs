using System;
using Codartis.SoftVis.Modeling;
using Microsoft.CodeAnalysis;

namespace Codartis.SoftVis.VisualStudioIntegration.App.Commands.ShellTriggered
{
    /// <summary>
    /// Adds the current symbol (the one at the caret) and its hierarchy (base type and subtypes) to the diagram.
    /// Shows the diagram if it was not visible.
    /// </summary>
    internal sealed class AddToDiagramWithHierarchyCommand : ShellTriggeredCommandBase
    {
        public AddToDiagramWithHierarchyCommand(IAppServices appServices)
            :base(VsctConstants.SoftVisCommandSetGuid, VsctConstants.AddToDiagramWithHierarchyCommand, appServices)
        {
        }

        public override async void Execute(object sender, EventArgs e)
        {
            var namedTypeSymbol = await HostWorkspaceServices.GetCurrentSymbol() as INamedTypeSymbol;
            if (namedTypeSymbol == null)
                return;

            var modelEntity = ModelServices.FindOrCreateModelEntity(namedTypeSymbol);
            if (modelEntity == null)
                return;

            ModelServices.ExtendModelWithRelatedEntities(modelEntity, RelatedEntitySpecifications.BaseType, recursive: true);
            ModelServices.ExtendModelWithRelatedEntities(modelEntity, RelatedEntitySpecifications.Subtype, recursive: true);

            DiagramServices.ShowModelEntityWithHierarchy(modelEntity);
            HostUiServices.DiagramHostWindow.Show();
            UiServices.FitDiagramToView();
        }
    }
}
