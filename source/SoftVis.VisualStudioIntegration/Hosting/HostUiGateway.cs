using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using Codartis.SoftVis.Util.UI.Wpf.Dialogs;
using Codartis.SoftVis.VisualStudioIntegration.UI;
using Microsoft.VisualStudio.Shell;
using Task = System.Threading.Tasks.Task;

namespace Codartis.SoftVis.VisualStudioIntegration.Hosting
{
    /// <summary>
    /// A gateway to access host UI operations.
    /// </summary>
    public sealed class HostUiGateway : IHostUiServices
    {
        private const string DialogTitle = "Quick Diagram Tool";

        private readonly IPackageServices _packageServices;

        public HostUiGateway(IPackageServices packageServices)
        {
            _packageServices = packageServices;
        }

        public Task ShowDiagramWindowAsync()
        {
            return _packageServices.ShowToolWindowAsync<DiagramHostToolWindow>();
        }

        public void ShowMessageBox(string message) => System.Windows.MessageBox.Show(message, DialogTitle);

        public string SelectSaveFilename(string title, string filter)
        {
            var saveFileDialog = new SaveFileDialog { Title = title, Filter = filter };
            saveFileDialog.ShowDialog();
            return saveFileDialog.FileName;
        }

        public async Task<ProgressDialog> CreateProgressDialogAsync(string text, int maxProgress = 0)
        {
            var hostMainWindow = await GetMainWindowAsync();
            return new ProgressDialog(hostMainWindow, DialogTitle, text, maxProgress);
        }

        public void Run(Func<Task> asyncMethod) => ThreadHelper.JoinableTaskFactory.RunAsync(asyncMethod);

        private async Task<Window> GetMainWindowAsync()
        {
            var hostEnvironmentService = _packageServices.GetHostEnvironmentService();

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