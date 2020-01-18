using System;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using Codartis.SoftVis.UI.Wpf.View;
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
        }

        private IVsWindowFrame WindowFrame
        {
            get
            {
                ThreadHelper.ThrowIfNotOnUIThread();
                return (IVsWindowFrame)Frame;
            }
        }

        public void Initialize(DiagramControl diagramControl)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            WindowFrame.SetProperty((int)__VSFPROPID.VSFPROPID_CmdUIGuid, PackageGuids.DiagramToolWindowGuidString);
            Content = diagramControl;
        }

        public void Show() => InvokeOnWindowFrame(i => i.Show());
        public void Hide() => InvokeOnWindowFrame(i => i.Hide());

        private void InvokeOnWindowFrame(Func<IVsWindowFrame, int> windowFrameAction)
        {
            ErrorHandler.ThrowOnFailure(windowFrameAction.Invoke(WindowFrame));
        }
    }
}
