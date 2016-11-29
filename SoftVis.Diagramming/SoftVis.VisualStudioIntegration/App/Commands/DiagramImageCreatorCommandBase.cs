using System;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Codartis.SoftVis.Util;
using Codartis.SoftVis.Util.UI.Wpf.Dialogs;

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
            var progressDialog = UiServices.ShowProgressDialog("Generating image..");
            progressDialog.Show();

            var cancellationToken = progressDialog.CancellationToken;
            var progress = new Progress<double>(i => progressDialog.SetProgressPercentage(i * .9));

            try
            {
                var bitmapSource = await UiServices.CreateDiagramImageAsync(cancellationToken, progress);
                await Task.Factory.StartSTA(() =>
                    ProcessDiagramImage(bitmapSource, imageProcessingAction, progressDialog, imageProcessingMessage), cancellationToken);
            }
            catch (OperationCanceledException)
            {
            }
            finally
            {
                progressDialog.Close();
            }
        }

        private static void ProcessDiagramImage(BitmapSource bitmapSource, Action<BitmapSource> imageProcessingAction, 
            ProgressDialog progressDialog, string imageProcessingMessage)
        {
            if (bitmapSource != null)
            {
                progressDialog.SetText(imageProcessingMessage);
                imageProcessingAction(bitmapSource);
                progressDialog.SetProgressPercentage(1);
            }
        }
    }
}