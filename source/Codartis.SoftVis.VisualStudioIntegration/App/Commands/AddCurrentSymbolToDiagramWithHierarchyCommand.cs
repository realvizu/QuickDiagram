using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.SoftVis.VisualStudioIntegration.Modeling;
using Codartis.Util;
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
        public AddCurrentSymbolToDiagramWithHierarchyCommand([NotNull] IAppServices appServices)
            : base(appServices)
        {
        }

        public override async Task ExecuteAsync()
        {
            var maybeModelNode = await TryAddCurrentSymbolToDiagramAsync();
            if (!maybeModelNode.HasValue)
                return;

            var addedNodeIds = await ExtendModelAndDiagramAsync(maybeModelNode.Value);
            if (!addedNodeIds.Any())
                return;

            await HostUiService.ShowDiagramWindowAsync();
            DiagramWindowService.FollowDiagramNodes(addedNodeIds);
        }

        [NotNull]
        [ItemNotNull]
        private async Task<IReadOnlyList<ModelNodeId>> ExtendModelAndDiagramAsync([NotNull] IModelNode node)
        {
            IReadOnlyList<ModelNodeId> nodeIds = Array.Empty<ModelNodeId>();

            using (var progressDialog = await HostUiService.CreateProgressDialogAsync("Extending model with entities:"))
            {
                await progressDialog.ShowWithDelayAsync();

                try
                {
                    await ExtendModelWithRelatedEntitiesAsync(node, progressDialog.CancellationToken, progressDialog.Progress);

                    progressDialog.Reset("Adding diagram nodes:");

                    nodeIds = await ExtendDiagramAsync(node.Id, progressDialog.CancellationToken, progressDialog.Progress);
                }
                catch (OperationCanceledException)
                {
                }
            }

            return nodeIds;
        }

        [NotNull]
        private async Task ExtendModelWithRelatedEntitiesAsync(
            [NotNull] IModelNode modelNode,
            CancellationToken cancellationToken,
            IIncrementalProgress progress)
        {
            await Task.WhenAll(
                RoslynModelService.ExtendModelWithRelatedNodesAsync(
                    modelNode,
                    DirectedModelRelationshipTypes.BaseType,
                    cancellationToken,
                    progress,
                    recursive: true),
                RoslynModelService.ExtendModelWithRelatedNodesAsync(
                    modelNode,
                    DirectedModelRelationshipTypes.Subtype,
                    cancellationToken,
                    progress,
                    recursive: true)
            );
        }

        [NotNull]
        [ItemNotNull]
        private async Task<IReadOnlyList<ModelNodeId>> ExtendDiagramAsync(
            ModelNodeId nodeId,
            CancellationToken cancellationToken,
            IIncrementalProgress progress)
        {
            return await Task.Run(
                () => ShowModelNodeWithHierarchy(nodeId, cancellationToken, progress).ToArray(),
                cancellationToken);
        }

        [NotNull]
        private IEnumerable<ModelNodeId> ShowModelNodeWithHierarchy(
            ModelNodeId nodeId,
            CancellationToken cancellationToken,
            IIncrementalProgress progress)
        {
            var model = RoslynModelService.ModelService.LatestModel;

            var baseTypeIds = model.GetRelatedNodes(nodeId, DirectedModelRelationshipTypes.BaseType, recursive: true).Select(i => i.Id);
            var subtypeIds = model.GetRelatedNodes(nodeId, DirectedModelRelationshipTypes.Subtype, recursive: true).Select(i => i.Id);
            var nodeIds = new[] { nodeId }.Union(baseTypeIds).Union(subtypeIds).ToList();

            DiagramService.AddNodes(nodeIds, cancellationToken, progress);

            return nodeIds;
        }
    }
}