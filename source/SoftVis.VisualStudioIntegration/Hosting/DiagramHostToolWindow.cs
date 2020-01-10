using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using Codartis.SoftVis.UI.Wpf.View;
using Microsoft.VisualStudio.Shell;

namespace Codartis.SoftVis.VisualStudioIntegration.Hosting
{
    /// <summary>
    /// Implements a Visual Studio tool window that hosts a diagram control.
    /// </summary>
    [Guid(PackageGuids.DiagramToolWindowGuidString)]
    [ProvideKeyBindingTable(PackageGuids.DiagramToolWindowGuidString, 115)]
    public sealed class DiagramHostToolWindow : ToolWindowPane
    {
        public DiagramHostToolWindow(DiagramControl diagramControl)
            : base(null)
        {
            Caption = "Quick Diagram";
            ToolBar = new CommandID(PackageGuids.SoftVisCommandSetGuid, PackageIds.ToolWindowToolbar);
            Content = diagramControl;
        }
    }
}
