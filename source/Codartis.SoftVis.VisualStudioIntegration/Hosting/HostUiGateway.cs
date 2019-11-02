using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
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
        private readonly IVisualStudioServices _visualStudioServices;
        //private DiagramHostToolWindow _diagramHostWindow;

        public HostUiGateway(IVisualStudioServices visualStudioServices)
        {
            _visualStudioServices = visualStudioServices;
        }

        // TODO: delete?
        //public void HostDiagram(ContentControl diagramControl)
        //{
        //    ThreadHelper.JoinableTaskFactory.Run(async delegate
        //    {
        //        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
        //        _diagramHostWindow = await _visualStudioServices.CreateToolWindowAsync<DiagramHostToolWindow>();
        //    });
        //}

        public Task ShowDiagramWindowAsync()
        {
            return _visualStudioServices.ShowToolWindowAsync<DiagramHostToolWindow>();
        }

        public async Task<Window> GetMainWindowAsync()
        {
            var hostService = _visualStudioServices.GetHostEnvironmentService();

            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            var parentWindowHandle = new IntPtr(hostService.MainWindow.HWnd);
            var hwndSource = HwndSource.FromHwnd(parentWindowHandle);
            return (Window)hwndSource?.RootVisual;
        }

        public void Run(Func<Task> asyncMethod) => ThreadHelper.JoinableTaskFactory.RunAsync(asyncMethod);
    }
}