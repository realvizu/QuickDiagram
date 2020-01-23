using System;
using System.Threading.Tasks;
using System.Windows;
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

        public async Task ShowDiagramWindowAsync()
        {
            var toolWindow = await _packageServices.GetToolWindowAsync();
            toolWindow.Show();
        }

        public async Task<ProgressDialog> CreateProgressDialogAsync(string text, int maxProgress = 0)
        {
            var hostMainWindow = await GetMainWindowAsync();
            return new ProgressDialog(hostMainWindow, DialogTitle, text, maxProgress);
        }

        public async Task RunAsync(Func<Task> asyncMethod)
        {
            await ThreadHelper.JoinableTaskFactory.RunAsync(asyncMethod);
        }

        private async Task<Window> GetMainWindowAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            var hostEnvironmentService = _packageServices.GetHostEnvironmentService();
            var parentWindowHandle = new IntPtr(hostEnvironmentService.MainWindow.HWnd);
            var hwndSource = HwndSource.FromHwnd(parentWindowHandle);
            var window = (Window)hwndSource?.RootVisual;

            if (window == null)
                throw new Exception("Could not get main window.");

            return window;
        }
    }
}