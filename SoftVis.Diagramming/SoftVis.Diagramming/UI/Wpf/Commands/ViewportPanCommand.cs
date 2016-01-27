using System;
using System.Windows;
using Codartis.SoftVis.UI.Common;

namespace Codartis.SoftVis.UI.Wpf.Commands
{
    /// <summary>
    /// A command that invokes a delegate for viewport pan.
    /// </summary>
    public class ViewportPanCommand : DelegateCommand<Vector, TransitionSpeed>
    {
        public ViewportPanCommand(Action<Vector, TransitionSpeed> execute, Func<Vector, TransitionSpeed, bool> canExecute = null)
            : base(execute, canExecute)
        { }

        public void Execute(Vector panVector,TransitionSpeed transitionSpeed)
            => ExecuteCore(panVector, transitionSpeed);
    }
}
