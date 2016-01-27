using System;
using System.Windows;
using Codartis.SoftVis.UI.Common;

namespace Codartis.SoftVis.UI.Wpf.Commands
{
    /// <summary>
    /// A command that invokes a delegate for viewport resize.
    /// </summary>
    public class ViewportResizeCommand : DelegateCommand<Size, TransitionSpeed>
    {
        public ViewportResizeCommand(Action<Size, TransitionSpeed> execute, Func<Size, TransitionSpeed, bool> canExecute = null)
            : base(execute, canExecute)
        { }

        public void Execute(Size newSize, TransitionSpeed transitionSpeed)
            => ExecuteCore(newSize, transitionSpeed);
    }
}
