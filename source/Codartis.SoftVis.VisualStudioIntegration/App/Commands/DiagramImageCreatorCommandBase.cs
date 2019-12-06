using System;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Codartis.Util;
using JetBrains.Annotations;

namespace Codartis.SoftVis.VisualStudioIntegration.App.Commands
{
    /// <summary>
    /// Abstract base class for commands that generate a diagram image.
    /// </summary>
    internal abstract class DiagramImageCreatorCommandBase : CommandBase
    {
        protected DiagramImageCreatorCommandBase([NotNull] IAppServices appServices)
            : base(appServices)
        {
        }

        [NotNull]
        protected async Task CreateAndProcessDiagramImageAsync(
            [NotNull] Action<BitmapSource> imageProcessingAction,
            [NotNull] string imageProcessingMessage)
        {
            // Using int.MaxValue for max progress because the real max value is not yet known.
            using (var progressDialog = await HostUiService.CreateProgressDialogAsync("Generating image..", int.MaxValue))
            {
                progressDialog.ShowProgressNumber = false;
                progressDialog.ShowWithDelay();

                try
                {
                    var bitmapSource = await DiagramWindowService.CreateDiagramImageAsync(
                        DiagramWindowService.ImageExportDpi.Value,
                        DiagramWindowService.ExportedImageMargin,
                        progressDialog.CancellationToken,
                        progressDialog.Progress,
                        progressDialog.MaxProgress);

                    progressDialog.Reset(imageProcessingMessage, showProgressNumber: false);
                    await Task.Factory.StartSTA(() => imageProcessingAction(bitmapSource), progressDialog.CancellationToken);
                }
                catch (OperationCanceledException)
                {
                }
                catch (OutOfMemoryException)
                {
                    HandleOutOfMemory();
                }
            }
        }

        private void HandleOutOfMemory()
        {
            HostUiService.ShowMessageBox("Cannot generate the image because it is too large. Please select a smaller DPI value.");
        }
    }
}