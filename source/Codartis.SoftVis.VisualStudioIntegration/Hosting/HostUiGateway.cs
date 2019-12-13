using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using Codartis.SoftVis.VisualStudioIntegration.UI;
using Codartis.Util.UI.Wpf.Dialogs;
using JetBrains.Annotations;
using Microsoft.VisualStudio.Shell;
using Task = System.Threading.Tasks.Task;

namespace Codartis.SoftVis.VisualStudioIntegration.Hosting
{
    /// <summary>
    /// A gateway to access host UI operations.
    /// </summary>
    public sealed class HostUiGateway : IHostUiService
    {
        private readonly IVisualStudioServices _visualStudioServices;

        public HostUiGateway(IVisualStudioServices visualStudioServices)
        {
            _visualStudioServices = visualStudioServices;
        }

        public Task ShowDiagramWindowAsync()
        {
            return _visualStudioServices.ShowToolWindowAsync<DiagramHostToolWindow>();
        }

        public void ShowMessageBox(string message) => System.Windows.MessageBox.Show(message, Constants.ToolName);

        public string SelectSaveFilename(string title, string filter)
        {
            var saveFileDialog = new SaveFileDialog { Title = title, Filter = filter };
            saveFileDialog.ShowDialog();
            return saveFileDialog.FileName;
        }

        public async Task<ProgressDialog> CreateProgressDialogAsync(string text, int maxProgress = 0)
        {
            var hostMainWindow = await GetMainWindowAsync();
            return new ProgressDialog(hostMainWindow, Constants.ToolName, text, maxProgress);
        }

        public void Run(Func<Task> asyncMethod) => ThreadHelper.JoinableTaskFactory.RunAsync(asyncMethod);

        [NotNull]
        [ItemNotNull]
        private async Task<Window> GetMainWindowAsync()
        {
            var hostEnvironmentService = _visualStudioServices.GetHostEnvironmentService();

            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            var parentWindowHandle = new IntPtr(hostEnvironmentService.MainWindow.HWnd);
            var hwndSource = HwndSource.FromHwnd(parentWindowHandle);
            var window = (Window)hwndSource?.RootVisual;

            if (window == null)
                throw new Exception("Could not get main window.");

            return window;
        }
    }
}