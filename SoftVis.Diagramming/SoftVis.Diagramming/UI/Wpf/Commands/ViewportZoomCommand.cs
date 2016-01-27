using System;
using System.Windows;
using Codartis.SoftVis.UI.Common;

namespace Codartis.SoftVis.UI.Wpf.Commands
{
    /// <summary>
    /// A command that invokes a delegate for viewport zoom.
    /// </summary>
    public class ViewportZoomCommand : DelegateCommand<double, Point, TransitionSpeed>
    {
        public ViewportZoomCommand(Action<double, Point, TransitionSpeed> execute, Func<double, Point, TransitionSpeed, bool> canExecute = null)
            : base(execute, canExecute)
        { }

        public void Execute(double zoomValue, Point zoomCenter, TransitionSpeed transitionSpeed)
            => ExecuteCore(zoomValue, zoomCenter, transitionSpeed);
    }
}
