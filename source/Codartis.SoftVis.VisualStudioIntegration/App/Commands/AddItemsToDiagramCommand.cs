using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.Util;
using JetBrains.Annotations;

namespace Codartis.SoftVis.VisualStudioIntegration.App.Commands
{
    /// <summary>
    /// Adds the given model items to the diagram and shows a progress dialog.
    /// </summary>
    internal sealed class AddItemsToDiagramCommand : CommandBase
    {
        [NotNull] private readonly IReadOnlyCollection<ModelNodeId> _modelNodeIds;
        private readonly bool _followWithViewport;

        public AddItemsToDiagramCommand(
            [NotNull] IAppServices appServices,
            [NotNull] IReadOnlyCollection<ModelNodeId> modelNodeIds,
            bool followWithViewport)
            : base(appServices)
        {
            _modelNodeIds = modelNodeIds;
            _followWithViewport = followWithViewport;
        }

        public override async Task ExecuteAsync()
        {
            await ShowProgressAndAddItemsAsync(_modelNodeIds);

            await HostUiService.ShowDiagramWindowAsync();

            if (_followWithViewport)
                DiagramWindowService.FollowDiagramNodes(_modelNodeIds);
        }

        [NotNull]
        private async Task ShowProgressAndAddItemsAsync([NotNull] IReadOnlyCollection<ModelNodeId> modelNodeIds)
        {
            using (var progressDialog = await HostUiService.CreateProgressDialogAsync("Adding model items:", modelNodeIds.Count))
            {
                progressDialog.ShowWithDelay();

                try
                {
                    await ShowEntitiesAsync(modelNodeIds, progressDialog.CancellationToken, progressDialog.Progress);
                }
                catch (OperationCanceledException)
                {
                }
            }
        }

        [NotNull]
        private async Task ShowEntitiesAsync(
            [NotNull] IReadOnlyCollection<ModelNodeId> modelEntities,
            CancellationToken cancellationToken,
            IIncrementalProgress progress)
        {
            await Task.Run(
                () => DiagramService.AddNodes(modelEntities, cancellationToken, progress),
                cancellationToken);
        }
    }
}