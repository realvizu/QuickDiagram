using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using Codartis.SoftVis.VisualStudioIntegration.UI;
using Microsoft.VisualStudio.Shell;

namespace Codartis.SoftVis.VisualStudioIntegration.Hosting
{
    /// <summary>
    /// A gateway to access host UI operations.
    /// </summary>
    public sealed class HostUiGateway : IHostUiServices
    {
        private readonly IPackageServices _packageServices;
        private DiagramHostToolWindow _diagramHostWindow;

        public HostUiGateway(IPackageServices packageServices)
        {
            _packageServices = packageServices;
        }

        public void HostDiagram(ContentControl diagramControl)
        {
            ThreadHelper.JoinableTaskFactory.Run(async delegate {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                _diagramHostWindow = await _packageServices.CreateToolWindowAsync<DiagramHostToolWindow>();
            });
        }

        public void ShowDiagramWindow()
        {
            _packageServices.ShowToolWindow<DiagramHostToolWindow>();
        }

        public async Task<Window> GetMainWindowAsync()
        {
            var hostService = _packageServices.GetHostEnvironmentService();

            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            var parentWindowHandle = new IntPtr(hostService.MainWindow.HWnd);
            var hwndSource = HwndSource.FromHwnd(parentWindowHandle);
            return (Window)hwndSource?.RootVisual;
        }
    }
}