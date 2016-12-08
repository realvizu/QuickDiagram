using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.Util;
using Codartis.SoftVis.VisualStudioIntegration.Modeling;

namespace Codartis.SoftVis.VisualStudioIntegration.App.Commands
{
    /// <summary>
    /// Adds the current symbol (the one at the caret) and its hierarchy (base type and subtypes) to the diagram.
    /// Shows the diagram if it was not visible.
    /// </summary>
    internal sealed class AddCurrentSymbolToDiagramWithHierarchyCommand : AsyncCommandBase
    {
        public AddCurrentSymbolToDiagramWithHierarchyCommand(IAppServices appServices)
            : base(appServices)
        {
        }

        public override async Task ExecuteAsync()
        {
            var modelEntity = await ModelServices.AddCurrentSymbolAsync();
            if (modelEntity == null)
                return;

            var diagramNodes = await ExtendModelAndDiagram(modelEntity);

            UiServices.ShowDiagramWindow();
            UiServices.ExecuteWhenUiIsIdle(() => UiServices.ZoomToDiagramNodes(diagramNodes));
        }

        private async Task<List<IDiagramNode>> ExtendModelAndDiagram(IRoslynBasedModelEntity modelEntity)
        {
            List<IDiagramNode> diagramNodes = null;

            var progressDialog = UiServices.CreateProgressDialog("Extending model with entities:");
            progressDialog.ShowWithDelayAsync();

            try
            {
                var cancellationToken = progressDialog.CancellationToken;

                await Task.Run(() => ExtendModelWithRelatedEntities(modelEntity, cancellationToken, progressDialog.Progress), cancellationToken);

                progressDialog.Reset("Adding diagram nodes:");

                diagramNodes = await Task.Run(() => ExtendDiagram(modelEntity, cancellationToken, progressDialog.Progress), cancellationToken);
            }
            catch (OperationCanceledException) { }
            finally { progressDialog.Close(); }

            return diagramNodes;
        }

        private void ExtendModelWithRelatedEntities(IModelEntity modelEntity, CancellationToken cancellationToken, IIncrementalProgress progress)
        {
            ModelServices.ExtendModelWithRelatedEntities(modelEntity, EntityRelationTypes.BaseType, cancellationToken, progress, recursive: true);
            ModelServices.ExtendModelWithRelatedEntities(modelEntity, EntityRelationTypes.Subtype, cancellationToken, progress, recursive: true);
        }

        private List<IDiagramNode> ExtendDiagram(IRoslynBasedModelEntity modelEntity, 
            CancellationToken cancellationToken, IIncrementalProgress progress)
        {
            return DiagramServices.ShowEntityWithHierarchy(modelEntity, cancellationToken, progress);
        }
    }
}
