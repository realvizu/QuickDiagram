using System;
using System.Linq;
using System.Runtime.InteropServices;
using Codartis.SoftVis.VisualStudioIntegration.ImageExport;
using Microsoft.VisualStudio.Shell;

namespace Codartis.SoftVis.VisualStudioIntegration.Commands
{
    /// <summary>
    /// Executed when the host requests the current value for the ImageDpiCombo, or the current value changes.
    /// </summary>
    internal sealed class ImageDpiComboCommand : CommandBase
    {
        public ImageDpiComboCommand(IPackageServices packageServices)
            : base(VsctConstants.SoftVisCommandSetGuid, VsctConstants.ImageDpiComboCommand, packageServices)
        { }

        public override void Execute(object sender, EventArgs e)
        {
            var oleMenuCmdEventArgs = (OleMenuCmdEventArgs)e;

            var diagramWindow = PackageServices.GetDiagramWindow();
            if (diagramWindow == null)
                throw new Exception("Unable to get DiagramWindow.");

            if (IsCurrentValueRequested(oleMenuCmdEventArgs))
            {
                var currentDpiName = diagramWindow.ImageExportDpi.Name;
                Marshal.GetNativeVariantForObject(currentDpiName, oleMenuCmdEventArgs.OutValue);
            }
            else if (IsSelectedValueModified(oleMenuCmdEventArgs))
            {
                var selectedString = Convert.ToString(oleMenuCmdEventArgs.InValue);
                var selectedDpi = Dpi.DpiChoices.First(i => i.Name == selectedString);
                diagramWindow.ImageExportDpi = selectedDpi;
            }
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
