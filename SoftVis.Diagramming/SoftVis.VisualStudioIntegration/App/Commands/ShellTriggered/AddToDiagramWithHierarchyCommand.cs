using System;
using Codartis.SoftVis.Modeling;

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
            var modelEntity = await ModelServices.AddCurrentSymbolAsync();
            if (modelEntity == null)
                return;

            ModelServices.ExtendModelWithRelatedEntities(modelEntity, EntityRelationTypes.BaseType, recursive: true);
            ModelServices.ExtendModelWithRelatedEntities(modelEntity, EntityRelationTypes.Subtype, recursive: true);

            DiagramServices.ShowModelEntityWithHierarchy(modelEntity);
            UiServices.ShowDiagramWindow();
            UiServices.FitDiagramToView();
        }
    }
}
