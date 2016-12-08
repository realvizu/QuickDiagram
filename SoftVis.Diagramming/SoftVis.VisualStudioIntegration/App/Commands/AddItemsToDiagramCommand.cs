using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Modeling;

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
            await ShowProgressAndAddItems(modelEntities);

            UiServices.ShowDiagramWindow();
            UiServices.EnsureDiagramVisible();
        }

        private async Task<List<IDiagramNode>> ShowProgressAndAddItems(List<IModelEntity> modelEntities)
        {
            List<IDiagramNode> diagramNodes = null;

            var progressDialog = UiServices.CreateProgressDialog("Adding model items:", modelEntities.Count);
            progressDialog.ShowWithDelayAsync();

            try
            {
                var cancellationToken = progressDialog.CancellationToken;
                diagramNodes= await Task.Run(() => DiagramServices.ShowEntities(modelEntities, cancellationToken, progressDialog.Progress), cancellationToken);
            }
            catch (OperationCanceledException) { }
            finally { progressDialog.Close(); }

            return diagramNodes;
        }
    }
}
