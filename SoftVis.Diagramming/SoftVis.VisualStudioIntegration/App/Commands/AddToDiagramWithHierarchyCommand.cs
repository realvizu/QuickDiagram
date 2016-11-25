using System.Threading.Tasks;
using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.VisualStudioIntegration.App.Commands
{
    /// <summary>
    /// Adds the current symbol (the one at the caret) and its hierarchy (base type and subtypes) to the diagram.
    /// Shows the diagram if it was not visible.
    /// </summary>
    internal sealed class AddToDiagramWithHierarchyCommand : AsyncCommandBase
    {
        public AddToDiagramWithHierarchyCommand(IAppServices appServices)
            :base(appServices)
        {
        }

        public override async Task ExecuteAsync()
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
