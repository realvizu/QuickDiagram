using System;
using System.Windows;
using Codartis.SoftVis.Util.UI.Wpf.Commands;

namespace Codartis.SoftVis.UI.Wpf.Commands
{
    /// <summary>
    /// A command that invokes a delegate with zoom parameters.
    /// </summary>
    public class ZoomDelegateCommand : DelegateCommand<ZoomDirection, double, Point>
    {
        public ZoomDelegateCommand(Action<ZoomDirection, double, Point> execute, Func<ZoomDirection, double, Point, bool> canExecute = null) 
            : base(execute, canExecute)
        { }

        public void Execute(ZoomDirection zoomDirection, double zoomValue, Point zoomCenter)
            => ExecuteCore(zoomDirection, zoomValue, zoomCenter);
    }
}
