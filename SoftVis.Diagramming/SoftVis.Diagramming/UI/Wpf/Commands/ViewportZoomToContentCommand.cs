using System;
using Codartis.SoftVis.UI.Common;

namespace Codartis.SoftVis.UI.Wpf.Commands
{
    /// <summary>
    /// A command that invokes a delegate for viewport zoom-to-content.
    /// </summary>
    public class ViewportZoomToContentCommand : DelegateCommand<TransitionSpeed>
    {
        public ViewportZoomToContentCommand(Action<TransitionSpeed> execute, Func<TransitionSpeed, bool> canExecute = null)
            : base(execute, canExecute)
        {
        }
    }
}
