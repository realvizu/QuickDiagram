using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Codartis.SoftVis.Util;

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
            await ShowProgressAndUpdateModelAsync();

            UiService.ShowDiagramWindow();
            UiService.ZoomToDiagram();
        }

        private async Task ShowProgressAndUpdateModelAsync()
        {
            using (var progressDialog = UiService.CreateProgressDialog("Updating model nodes:"))
            {
                progressDialog.ShowWithDelayAsync();

                try
                {
                    await UpdateModelAsync(progressDialog.CancellationToken, progressDialog.Progress);
                }
                catch (OperationCanceledException)
                {
                }
            }
        }

        private async Task UpdateModelAsync(CancellationToken cancellationToken, IIncrementalProgress progress)
        {
            var visibleModelNodeIds = DiagramServices.Diagram.Nodes.Select(i => i.ModelNode.Id);

            await Task.Run(
                () => ModelService.UpdateFromSource(visibleModelNodeIds, cancellationToken, progress),
                cancellationToken);
        }
    }
}
