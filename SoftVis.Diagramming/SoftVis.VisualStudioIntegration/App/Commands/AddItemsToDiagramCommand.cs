using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.VisualStudioIntegration.App.Commands
{
    /// <summary>
    /// Adds the given model items to the diagram and shows a progress dialog.
    /// </summary>
    internal sealed class AddItemsToDiagramCommand : AsyncCommandWithParameterBase<List<IModelItem>>
    {
        public AddItemsToDiagramCommand(IAppServices appServices)
            : base(appServices)
        {
        }

        public override async Task ExecuteAsync(List<IModelItem> modelItems)
        {
            await ShowProgressAndAddItems(modelItems);

            UiServices.ShowDiagramWindow();
            UiServices.EnsureDiagramContentVisible();
        }

        private async Task ShowProgressAndAddItems(List<IModelItem> modelItems)
        {
            var progressDialog = UiServices.CreateProgressDialog("Adding model items:", modelItems.Count);
            progressDialog.ShowWithDelayAsync();

            try
            {
                var cancellationToken = progressDialog.CancellationToken;
                await Task.Run(() => DiagramServices.ShowItemsWithProgress(modelItems, cancellationToken, progressDialog.Progress), cancellationToken);
            }
            catch (OperationCanceledException)
            {
            }
            finally
            {
                progressDialog.Close();
            }
        }
    }
}
