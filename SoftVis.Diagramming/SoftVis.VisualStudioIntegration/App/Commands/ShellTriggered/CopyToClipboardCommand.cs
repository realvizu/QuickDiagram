using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using Codartis.SoftVis.Util;

namespace Codartis.SoftVis.VisualStudioIntegration.App.Commands.ShellTriggered
{
    /// <summary>
    /// Copies the current diagram to the clipboard.
    /// </summary>
    internal sealed class CopyToClipboardCommand : ShellTriggeredCommandBase
    {
        public CopyToClipboardCommand(IAppServices appServices)
            : base(VsctConstants.SoftVisCommandSetGuid, VsctConstants.CopyToClipboradCommand, appServices)
        { }

        public override void Execute(object sender, EventArgs e)
        {
            var progressDialog = UiServices.ShowProgressDialog("Generating image..");
            progressDialog.Show();

            var progress = new Progress<double>(i => progressDialog.SetProgress(i * .9));

            UiServices.CreateDiagramImageAsync(progressDialog.CancellationToken, progress)
                .ContinueInCurrentContext(SetImageToClipboard)
                .ContinueInCurrentContext(i => progressDialog.Close());
        }

        private void SetImageToClipboard(Task<BitmapSource> task)
        {
            try
            {
                if (task.Status == TaskStatus.RanToCompletion && task.Result != null)
                    Clipboard.SetImage(task.Result);
            }
            catch (OutOfMemoryException)
            {
                HandleOutOfMemory();
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Exception in SetImageToClipboard: {e}");
                throw;
            }
        }

        private void HandleOutOfMemory()
        {
            UiServices.MessageBox("Cannot create the image because it is too large. Please select a smaller DPI value.");
        }
    }
}
