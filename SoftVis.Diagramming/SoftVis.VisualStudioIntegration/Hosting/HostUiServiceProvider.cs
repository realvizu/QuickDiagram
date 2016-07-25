using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using Codartis.SoftVis.VisualStudioIntegration.App;
using Microsoft.VisualStudio.Shell;

namespace Codartis.SoftVis.VisualStudioIntegration.Hosting
{
    /// <summary>
    /// Implements the host's UI operations that can be used by the application.
    /// </summary>
    public sealed class HostUiServiceProvider : IHostUiServices
    {
        private readonly IPackageServices _packageServices;

        public IHostWindow DiagramHostWindow { get; }

        public HostUiServiceProvider(IPackageServices packageServices)
        {
            _packageServices = packageServices;
            DiagramHostWindow = _packageServices.CreateToolWindow<DiagramHostToolWindow>();
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