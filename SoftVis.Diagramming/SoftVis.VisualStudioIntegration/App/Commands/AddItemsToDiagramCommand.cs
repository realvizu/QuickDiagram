using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.Util;

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
            UiServices.FitDiagramToView();
        }

        private async Task ShowProgressAndAddItems(List<IModelItem> modelItems)
        {
            var progressDialog = UiServices.ShowProgressDialog("Adding model items...");
            progressDialog.Show();

            var cancellationToken = progressDialog.CancellationToken;
            var progress = new PercentCalculatorProgress(i => progressDialog.SetProgressPercentage(i), modelItems.Count);

            try
            {
                await Task.Run(() => DiagramServices.ShowItemsWithProgress(modelItems, cancellationToken, progress), cancellationToken);
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
