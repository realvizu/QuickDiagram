using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using Codartis.SoftVis.VisualStudioIntegration.UI;

namespace Codartis.SoftVis.VisualStudioIntegration.Hosting
{
    /// <summary>
    /// A gateway to access host UI operations.
    /// </summary>
    public sealed class HostUiGateway : IHostUiServices
    {
        private readonly IPackageServices _packageServices;
        private readonly DiagramHostToolWindow _diagramHostWindow;

        public HostUiGateway(IPackageServices packageServices)
        {
            _packageServices = packageServices;
            _diagramHostWindow = _packageServices.CreateToolWindow<DiagramHostToolWindow>();
        }

        public void HostDiagram(ContentControl diagramControl)
        {
            _diagramHostWindow.Initialize("Diagram", diagramControl);
        }

        public void ShowDiagramWindow()
        {
            _diagramHostWindow.Show();
        }

        public Window GetMainWindow()
        {
            var hostService = _packageServices.GetHostEnvironmentService();
            var parentWindowHandle = new IntPtr(hostService.MainWindow.HWnd);
            var hwndSource = HwndSource.FromHwnd(parentWindowHandle);
            return (Window)hwndSource?.RootVisual;
        }
    }
}