using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Util;
using Codartis.SoftVis.VisualStudioIntegration.Modeling;
using JetBrains.Annotations;

namespace Codartis.SoftVis.VisualStudioIntegration.App.Commands
{
    /// <summary>
    /// Adds the current symbol (the one at the caret) and its hierarchy (base type and subtypes) to the diagram.
    /// Shows the diagram if it was not visible.
    /// </summary>
    [UsedImplicitly]
    internal sealed class AddCurrentSymbolToDiagramWithHierarchyCommand : AddCurrentSymbolToDiagramCommandBase
    {
        public AddCurrentSymbolToDiagramWithHierarchyCommand(IAppServices appServices)
            : base(appServices)
        {
        }

        public override async Task ExecuteAsync()
        {
            var modelEntity = await ModelService.AddCurrentSymbolAsync();
            if (modelEntity == null)
                return;

            var diagramNodes = await ExtendModelAndDiagramAsync(modelEntity);
            if (diagramNodes == null)
                return;

            await UiService.ShowDiagramWindowAsync();
            UiService.FollowDiagramNodes(diagramNodes);
        }

        private async Task<IReadOnlyList<IDiagramNode>> ExtendModelAndDiagramAsync(IRoslynModelNode modelNode)
        {
            IReadOnlyList<IDiagramNode> diagramNodes = null;

            using (var progressDialog = await UiService.CreateProgressDialogAsync("Extending model with entities:"))
            {
                await progressDialog.ShowWithDelayAsync();

                try
                {
                    await ExtendModelWithRelatedEntitiesAsync(modelNode, progressDialog.CancellationToken, progressDialog.Progress);

                    progressDialog.Reset("Adding diagram nodes:");

                    diagramNodes = await ExtendDiagramAsync(modelNode, progressDialog.CancellationToken, progressDialog.Progress);
                }
                catch (OperationCanceledException)
                {
                }
            }

            return diagramNodes;
        }

        private async Task ExtendModelWithRelatedEntitiesAsync(IRoslynModelNode modelEntity,
            CancellationToken cancellationToken, IIncrementalProgress progress)
        {
            await Task.WhenAll(
                ModelService.ExtendModelWithRelatedNodesAsync(modelEntity, DirectedRelationshipTypes.BaseType, cancellationToken, progress, recursive: true),
                ModelService.ExtendModelWithRelatedNodesAsync(modelEntity, DirectedRelationshipTypes.Subtype, cancellationToken, progress, recursive: true)
            );
        }

        private async Task<IReadOnlyList<IDiagramNode>> ExtendDiagramAsync(IRoslynModelNode modelNode,
            CancellationToken cancellationToken, IIncrementalProgress progress)
        {
            return await Task.Run(
                () => DiagramServices.ShowModelNodeWithHierarchy(modelNode, cancellationToken, progress).ToArray(),
                cancellationToken);
        }
    }
}