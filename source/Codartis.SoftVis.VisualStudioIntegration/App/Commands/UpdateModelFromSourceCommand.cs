using System;
using System.Threading.Tasks;
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
        public UpdateModelFromSourceCommand([NotNull] IAppServices appServices)
            : base(appServices)
        {
        }

        public override async Task ExecuteAsync()
        {
            await ShowProgressAndUpdateModelAsync();
            await HostUiService.ShowDiagramWindowAsync();
            DiagramWindowService.ZoomToDiagram();
        }

        [NotNull]
        private async Task ShowProgressAndUpdateModelAsync()
        {
            using (var progressDialog = await HostUiService.CreateProgressDialogAsync("Updating model nodes:"))
            {
                progressDialog.ShowWithDelay();

                try
                {
                    await RoslynModelService.UpdateFromSourceAsync(progressDialog.CancellationToken, progressDialog.Progress);
                }
                catch (OperationCanceledException)
                {
                }
            }
        }
    }
}