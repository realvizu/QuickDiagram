using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using System.Windows.Controls;
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
        private readonly ContentPresenter _contentPresenter;

        public DiagramHostToolWindow()
            : base(null)
        {
            Caption = "Quick Diagram";
            ToolBar = new CommandID(PackageGuids.SoftVisCommandSetGuid, PackageIds.ToolWindowToolbar);

            // Must assign a control to Content otherwise the tool window creation throws COMException (E_UNEXPECTED)
            // so we use an empty ContentPresenter and set its content to the DiagramControl later.
            _contentPresenter = new ContentPresenter();
            Content = _contentPresenter;
        }

        public void Initialize(DiagramControl diagramControl)
        {
            if (_contentPresenter.Content != null)
            {
                // Already initialized.
                return;
            }

            ThreadHelper.ThrowIfNotOnUIThread();
            ((IVsWindowFrame)Frame).SetProperty((int)__VSFPROPID.VSFPROPID_CmdUIGuid, PackageGuids.DiagramToolWindowGuidString);
            _contentPresenter.Content = diagramControl;
        }

        public void Show()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var windowFrame = (IVsWindowFrame)Frame;
            ErrorHandler.ThrowOnFailure(windowFrame.Show());
        }
    }
}