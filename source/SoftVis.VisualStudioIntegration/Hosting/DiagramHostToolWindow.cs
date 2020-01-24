using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace Codartis.SoftVis.VisualStudioIntegration.Hosting
{
    /// <summary>
    /// Implements a Visual Studio tool window that hosts a diagram control.
    /// </summary>
    [Guid(PackageGuids.DiagramToolWindowGuidString)]
    [ProvideKeyBindingTable(PackageGuids.DiagramToolWindowGuidString, 115)]
    public sealed class DiagramHostToolWindow : ToolWindowPane
    {
        public DiagramHostToolWindow()
            : base(null)
        {
            Caption = "Quick Diagram";
            ToolBar = new CommandID(PackageGuids.SoftVisCommandSetGuid, PackageIds.ToolWindowToolbar);

            // Ugly hack to work around the problem that no parameter can be passed to this ctor.
            Content = SoftVisPackage.DiagramToolApplication.UiServices.DiagramControl;
        }

        public void Show()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var windowFrame = (IVsWindowFrame)Frame;
            ErrorHandler.ThrowOnFailure(windowFrame.Show());
        }
    }
}