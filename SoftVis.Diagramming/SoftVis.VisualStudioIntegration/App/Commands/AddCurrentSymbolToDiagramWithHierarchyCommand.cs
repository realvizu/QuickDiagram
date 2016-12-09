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

            var diagramNodes = await ExtendModelAndDiagramAsync(modelEntity);
            if (diagramNodes == null)
                return;

            UiServices.ShowDiagramWindow();
            UiServices.ExecuteWhenUiIsIdle(() => UiServices.ZoomToDiagramNodes(diagramNodes));
        }

        private async Task<List<IDiagramNode>> ExtendModelAndDiagramAsync(IRoslynBasedModelEntity modelEntity)
        {
            List<IDiagramNode> diagramNodes = null;

            using (var progressDialog = UiServices.CreateProgressDialog("Extending model with entities:"))
            {
                progressDialog.ShowWithDelayAsync();

                try
                {
                    await ExtendModelWithRelatedEntitiesAsync(modelEntity, progressDialog.CancellationToken, progressDialog.Progress);

                    progressDialog.Reset("Adding diagram nodes:");

                    diagramNodes = await ExtendDiagramAsync(modelEntity, progressDialog.CancellationToken, progressDialog.Progress);
                }
                catch (OperationCanceledException)
                {
                }
            }

            return diagramNodes;
        }

        private async Task ExtendModelWithRelatedEntitiesAsync(IModelEntity modelEntity, 
            CancellationToken cancellationToken, IIncrementalProgress progress)
        {
            await Task.Run(() => ExtendModelWithRelatedEntities(modelEntity, cancellationToken, progress), cancellationToken);
        }

        private void ExtendModelWithRelatedEntities(IModelEntity modelEntity, CancellationToken cancellationToken, IIncrementalProgress progress)
        {
            ModelServices.ExtendModelWithRelatedEntities(modelEntity, EntityRelationTypes.BaseType, cancellationToken, progress, recursive: true);
            ModelServices.ExtendModelWithRelatedEntities(modelEntity, EntityRelationTypes.Subtype, cancellationToken, progress, recursive: true);
        }

        private async Task<List<IDiagramNode>> ExtendDiagramAsync(IRoslynBasedModelEntity modelEntity, 
            CancellationToken cancellationToken, IIncrementalProgress progress)
        {
            return await Task.Run(() => DiagramServices.ShowEntityWithHierarchy(modelEntity, cancellationToken, progress), cancellationToken);
        }
    }
}
