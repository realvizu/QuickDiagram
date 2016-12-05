using System;
using System.Linq;
using System.Threading.Tasks;

namespace Codartis.SoftVis.VisualStudioIntegration.App.Commands
{
    /// <summary>
    /// Updates the model (and consequently the diagram) to reflect the current state of the source code.
    /// Removes entities and relationships that no longer exist in code.
    /// </summary>
    internal sealed class UpdateModelFromSourceCommand : AsyncCommandBase
    {
        public UpdateModelFromSourceCommand(IAppServices appServices)
            : base(appServices)
        {
        }

        public override async Task ExecuteAsync()
        {
            await ShowProgressAndUpdateModel();

            UiServices.ShowDiagramWindow();
            UiServices.EnsureDiagramContentVisible();
        }

        private async Task ShowProgressAndUpdateModel()
        {
            var progressDialog = UiServices.CreateProgressDialog("Updating model entities:", ModelServices.Model.Entities.Count());
            progressDialog.ShowWithDelayAsync();

            try
            {
                var cancellationToken = progressDialog.CancellationToken;

                await Task.Run(() => ModelServices.UpdateFromCode(cancellationToken, progressDialog.Progress), cancellationToken);

                progressDialog.Reset("Updating diagram nodes:", DiagramServices.Nodes.Count());

                await Task.Run(() => DiagramServices.UpdateFromCode(cancellationToken, progressDialog.Progress), cancellationToken);
            }
            catch (OperationCanceledException) { }
            finally { progressDialog.Close(); }
        }
    }
}
