using System;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Codartis.SoftVis.Util;

namespace Codartis.SoftVis.VisualStudioIntegration.App.Commands
{
    /// <summary>
    /// Abstract base class for commands that generate a diagram image.
    /// </summary>
    internal abstract class DiagramImageCreatorCommandBase : AsyncCommandBase
    {
        protected DiagramImageCreatorCommandBase(IAppServices appServices)
            : base(appServices)
        {
        }

        protected async Task CreateAndProcessDiagramImageAsync(Action<BitmapSource> imageProcessingAction, string imageProcessingMessage)
        {
            // Using int.MaxValue for max progress because the real max value is not yet known.
            using (var progressDialog = await HostUiServices.CreateProgressDialogAsync("Generating image..", int.MaxValue))
            {
                progressDialog.ShowProgressNumber = false;
                progressDialog.ShowWithDelayAsync();

                try
                {
                    var bitmapSource = await UiServices.CreateDiagramImageAsync(progressDialog.CancellationToken, 
                        progressDialog.Progress, progressDialog.MaxProgress);

                    if (bitmapSource != null)
                    {
                        progressDialog.Reset(imageProcessingMessage, showProgressNumber: false);
                        await Task.Factory.StartSTA(() => imageProcessingAction(bitmapSource), progressDialog.CancellationToken);
                    }
                }
                catch (OperationCanceledException)
                {
                }
            }
        }
    }
}