using System;
using Codartis.Util.UI.Wpf.Commands;

namespace Codartis.SoftVis.UI.Wpf.Commands
{
    /// <summary>
    /// A command that invokes a delegate with a double parameter.
    /// </summary>
    public class DoubleDelegateCommand : DelegateCommand<double>
    {
        public DoubleDelegateCommand(Action<double> execute, Func<double, bool> canExecute = null) 
            : base(execute, canExecute)
        {
        }
    }
}
