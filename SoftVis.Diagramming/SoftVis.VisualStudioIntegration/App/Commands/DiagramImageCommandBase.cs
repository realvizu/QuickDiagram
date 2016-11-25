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
    internal abstract class DiagramImageCommandBase : ParameterlessCommandBase
    {
        protected DiagramImageCommandBase(IAppServices appServices)
            : base(appServices)
        {
        }

        protected void CreateDiagramImage(Action<BitmapSource> imageProcessingAction, string imageProcessingMessage)
        {
            var progressDialog = UiServices.ShowProgressDialog("Generating image..");
            progressDialog.Show();

            var progress = new Progress<double>(i => progressDialog.SetProgress(i * .9));

            UiServices.CreateDiagramImageAsync(progressDialog.CancellationToken, progress)
                .ContinueInCurrentContext(i => ProcessDiagramImage(i, imageProcessingAction, progressDialog, imageProcessingMessage))
                .ContinueInCurrentContext(i => progressDialog.Close());
        }

        private static void ProcessDiagramImage(Task<BitmapSource> task, Action<BitmapSource> action,
            ProgressDialog progressDialog, string progressText)
        {
            if (task.Status == TaskStatus.RanToCompletion && task.Result != null)
            {
                progressDialog.SetText(progressText);
                action(task.Result);
            }
        }
    }
}