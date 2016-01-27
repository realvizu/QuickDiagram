using System;
using System.Windows;

namespace Codartis.SoftVis.UI.Wpf.Commands
{
    /// <summary>
    /// A command that invokes a delegate with a Vector parameter.
    /// </summary>
    public class VectorCommand : DelegateCommand<Vector>
    {
        public VectorCommand(Action<Vector> execute, Func<Vector, bool> canExecute = null) 
            : base(execute, canExecute)
        {
        }
    }
}
