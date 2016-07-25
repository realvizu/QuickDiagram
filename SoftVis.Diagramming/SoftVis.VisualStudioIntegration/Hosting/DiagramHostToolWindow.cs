using System;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using System.Windows.Controls;
using Codartis.SoftVis.VisualStudioIntegration.App;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace Codartis.SoftVis.VisualStudioIntegration.Hosting
{
    /// <summary>
    /// Implements a Visual Studio tool window that hosts a diagram control.
    /// </summary>
    [Guid("02d1f8b9-d0a0-4ccb-9687-e6f0f781ad9e")]
    public sealed class DiagramHostToolWindow : ToolWindowPane, IHostWindow
    {
        private readonly ContentPresenter _contentPresenter;

        public DiagramHostToolWindow() 
            : base(null)
        {
            ToolBar = new CommandID(VsctConstants.SoftVisCommandSetGuid, VsctConstants.ToolWindowToolbar);
            _contentPresenter = new ContentPresenter();
            Content = _contentPresenter;
        }

        public void Initialize(string caption, object control)
        {
            Caption = caption;
            _contentPresenter.Content = control;
        }

        public void Show() => InvokeOnWindowFrame(i => i.Show());
        public void Hide() => InvokeOnWindowFrame(i => i.Hide());

        private void InvokeOnWindowFrame(Func<IVsWindowFrame, int> windowFrameAction)
        {
            var windowFrame = (IVsWindowFrame)Frame;
            ErrorHandler.ThrowOnFailure(windowFrameAction.Invoke(windowFrame));
        }
    }
}
