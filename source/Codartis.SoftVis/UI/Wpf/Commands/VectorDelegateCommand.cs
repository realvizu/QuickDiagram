using System;
using System.Windows;
using Codartis.Util.UI.Wpf.Commands;

namespace Codartis.SoftVis.UI.Wpf.Commands
{
    /// <summary>
    /// A command that invokes a delegate with a Vector parameter.
    /// </summary>
    public class VectorDelegateCommand : DelegateCommand<Vector>
    {
        public VectorDelegateCommand(Action<Vector> execute, Func<Vector, bool> canExecute = null) 
            : base(execute, canExecute)
        {
        }
    }
}
