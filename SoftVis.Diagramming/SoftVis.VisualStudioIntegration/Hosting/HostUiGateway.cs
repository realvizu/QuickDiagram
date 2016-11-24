using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
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

        public void AddMenuCommand(Guid commandSet, int commandId, EventHandler commandDelegate)
        {
            var menuCommandId = new CommandID(commandSet, commandId);
            var menuItem = new OleMenuCommand(commandDelegate, menuCommandId);

            _packageServices.GetMenuCommandService().AddCommand(menuItem);
        }

        public void FillCombo(EventArgs e, IEnumerable<string> items)
        {
            var oleMenuCmdEventArgs = (OleMenuCmdEventArgs)e;

            var outValue = oleMenuCmdEventArgs.OutValue;
            if (outValue != IntPtr.Zero)
                Marshal.GetNativeVariantForObject(items, outValue);
        }

        public ComboCommandType GetComboCommandType(EventArgs e)
        {
            var oleMenuCmdEventArgs = (OleMenuCmdEventArgs)e;

            if (IsCurrentValueRequested(oleMenuCmdEventArgs))
                return ComboCommandType.CurrentItemRequested;

            if (IsSelectedValueModified(oleMenuCmdEventArgs))
                return ComboCommandType.SelectedItemChanged;

            return ComboCommandType.Unknown;
        }

        public void SetCurrentComboItem(EventArgs e, string item)
        {
            var oleMenuCmdEventArgs = (OleMenuCmdEventArgs)e;
            Marshal.GetNativeVariantForObject(item, oleMenuCmdEventArgs.OutValue);
        }

        public string GetSelectedComboItem(EventArgs e)
        {
            var oleMenuCmdEventArgs = (OleMenuCmdEventArgs)e;
            return Convert.ToString(oleMenuCmdEventArgs.InValue);
        }

        private static bool IsCurrentValueRequested(OleMenuCmdEventArgs oleMenuCmdEventArgs)
        {
            return oleMenuCmdEventArgs.OutValue != IntPtr.Zero;
        }

        private static bool IsSelectedValueModified(OleMenuCmdEventArgs oleMenuCmdEventArgs)
        {
            return oleMenuCmdEventArgs.InValue != null;
        }
    }
}