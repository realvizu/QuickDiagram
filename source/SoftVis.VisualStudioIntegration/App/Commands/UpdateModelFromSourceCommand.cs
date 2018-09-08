using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Codartis.SoftVis.Util;
using JetBrains.Annotations;

namespace Codartis.SoftVis.VisualStudioIntegration.App.Commands
{
    /// <summary>
    /// Updates the model (and consequently the diagram) to reflect the current state of the source code.
    /// Removes entities and relationships that no longer exist in code.
    /// </summary>
    [UsedImplicitly]
    internal sealed class UpdateModelFromSourceCommand : CommandBase
    {
        public UpdateModelFromSourceCommand(IAppServices appServices)
            : base(appServices)
        {
        }

        public override async Task ExecuteAsync()
        {
            await ShowProgressAndUpdateModelAsync();
            await UiService.ShowDiagramWindowAsync();
            UiService.ZoomToDiagram();
        }

        private async Task ShowProgressAndUpdateModelAsync()
        {
            using (var progressDialog = await UiService.CreateProgressDialogAsync("Updating model nodes:"))
            {
                await progressDialog.ShowWithDelayAsync();

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

            await ModelService.UpdateFromSourceAsync(visibleModelNodeIds, cancellationToken, progress);
        }
    }
}
