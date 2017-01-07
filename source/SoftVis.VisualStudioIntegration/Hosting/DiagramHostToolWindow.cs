using System;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using System.Windows.Controls;
using Codartis.SoftVis.VisualStudioIntegration.UI;
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
            ToolBar = new CommandID(PackageGuids.SoftVisCommandSetGuid, PackageIds.ToolWindowToolbar);
            _contentPresenter = new ContentPresenter();
            Content = _contentPresenter;
        }

        private IVsWindowFrame WindowFrame => (IVsWindowFrame)Frame;

        public FrameMode FrameMode
        {
            get
            {
                object currentFrameMode;
                WindowFrame.GetProperty((int)__VSFPROPID.VSFPROPID_FrameMode, out currentFrameMode);
                return FrameModeTranslator.Translate(currentFrameMode);
            }
        }

        public void Initialize(string caption, object control)
        {
            WindowFrame.SetProperty((int)__VSFPROPID.VSFPROPID_CmdUIGuid, PackageGuids.DiagramToolWindowGuidString);
            Caption = caption;
            _contentPresenter.Content = control;
        }

        public void Show() => InvokeOnWindowFrame(i => i.Show());
        public void Hide() => InvokeOnWindowFrame(i => i.Hide());

        private void InvokeOnWindowFrame(Func<IVsWindowFrame, int> windowFrameAction)
        {
            ErrorHandler.ThrowOnFailure(windowFrameAction.Invoke(WindowFrame));
        }
    }
}
