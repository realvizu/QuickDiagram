using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.Util;

namespace Codartis.SoftVis.VisualStudioIntegration.App.Commands
{
    /// <summary>
    /// Adds the given model items to the diagram and shows a progress dialog.
    /// </summary>
    internal sealed class AddItemsToDiagramCommand : AsyncCommandWithParameterBase<List<IModelEntity>>
    {
        public AddItemsToDiagramCommand(IAppServices appServices)
            : base(appServices)
        {
        }

        public override async Task ExecuteAsync(List<IModelEntity> modelEntities)
        {
            var diagramNodes = await ShowProgressAndAddItemsAsync(modelEntities);

            UiServices.ShowDiagramWindow();

            if (diagramNodes.Count > 1)
                UiServices.FollowDiagramNodes(diagramNodes);
        }

        private async Task<List<IDiagramNode>> ShowProgressAndAddItemsAsync(List<IModelEntity> modelEntities)
        {
            List<IDiagramNode> diagramNodes = null;

            using (var progressDialog = UiServices.CreateProgressDialog("Adding model items:", modelEntities.Count))
            {
                progressDialog.ShowWithDelayAsync();

                try
                {
                    diagramNodes = await ShowEntitiesAsync(modelEntities, progressDialog.CancellationToken, progressDialog.Progress);
                }
                catch (OperationCanceledException)
                {
                }
            }

            return diagramNodes;
        }

        private async Task<List<IDiagramNode>> ShowEntitiesAsync(List<IModelEntity> modelEntities,
            CancellationToken cancellationToken, IIncrementalProgress progress)
        {
            return await Task.Run(() => DiagramServices.ShowEntities(modelEntities, cancellationToken, progress), cancellationToken);
        }
    }
}
