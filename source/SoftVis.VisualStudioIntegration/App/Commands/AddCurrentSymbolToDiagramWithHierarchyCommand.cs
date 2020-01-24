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
    internal sealed class AddCurrentSymbolToDiagramWithHierarchyCommand : AddCurrentSymbolToDiagramCommandBase
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

            await HostUiServices.ShowDiagramWindowAsync();

            var diagramNodes = await ExtendModelAndDiagramAsync(modelEntity);
            if (diagramNodes == null)
                return;

            UiServices.FollowDiagramNodes(diagramNodes);
        }

        private async Task<IReadOnlyList<IDiagramNode>> ExtendModelAndDiagramAsync(IRoslynBasedModelEntity modelEntity)
        {
            IReadOnlyList<IDiagramNode> diagramNodes = null;

            using (var progressDialog = await HostUiServices.CreateProgressDialogAsync("Extending model with entities:"))
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

        private async Task ExtendModelWithRelatedEntitiesAsync(
            IModelEntity modelEntity,
            CancellationToken cancellationToken,
            IIncrementalProgress progress)
        {
            await ModelServices.ExtendModelWithRelatedEntitiesAsync(modelEntity, EntityRelationTypes.BaseType, cancellationToken, progress, recursive: true);
            await ModelServices.ExtendModelWithRelatedEntitiesAsync(modelEntity, EntityRelationTypes.Subtype, cancellationToken, progress, recursive: true);
        }

        private async Task<IReadOnlyList<IDiagramNode>> ExtendDiagramAsync(
            IRoslynBasedModelEntity modelEntity,
            CancellationToken cancellationToken,
            IIncrementalProgress progress)
        {
            return await Task.Run(() => DiagramServices.ShowEntityWithHierarchy(modelEntity, cancellationToken, progress), cancellationToken);
        }
    }
}