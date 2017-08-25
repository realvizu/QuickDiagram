using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Util;
using Codartis.SoftVis.VisualStudioIntegration.Modeling;

namespace Codartis.SoftVis.VisualStudioIntegration.App.Commands
{
    /// <summary>
    /// Adds the given model items to the diagram and shows a progress dialog.
    /// </summary>
    internal sealed class AddItemsToDiagramCommand : AsyncCommandWithParameterBase<IReadOnlyList<IRoslynModelNode>, bool>
    {
        public AddItemsToDiagramCommand(IAppServices appServices)
            : base(appServices)
        {
        }

        public override async Task ExecuteAsync(IReadOnlyList<IRoslynModelNode> modelEntities, bool followWithViewport)
        {
            var diagramNodes = await ShowProgressAndAddItemsAsync(modelEntities);

            UiServices.ShowDiagramWindow();

            if (followWithViewport)
                UiServices.FollowDiagramNodes(diagramNodes);
        }

        private async Task<IReadOnlyList<IDiagramNode>> ShowProgressAndAddItemsAsync(IReadOnlyList<IRoslynModelNode> modelEntities)
        {
            IReadOnlyList<IDiagramNode> diagramNodes = null;

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

        private async Task<IReadOnlyList<IDiagramNode>> ShowEntitiesAsync(IReadOnlyList<IRoslynModelNode> modelEntities,
            CancellationToken cancellationToken, IIncrementalProgress progress)
        {
            return await Task.Run(() => DiagramServices.ShowModelNodes(modelEntities, cancellationToken, progress), cancellationToken);
        }
    }
}
