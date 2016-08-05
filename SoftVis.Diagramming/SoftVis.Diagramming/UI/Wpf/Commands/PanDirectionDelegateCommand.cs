using System;
using Codartis.SoftVis.Util.UI.Wpf;

namespace Codartis.SoftVis.UI.Wpf.Commands
{
    /// <summary>
    /// A command that invokes a delegate with a PanDirection parameter.
    /// </summary>
    public class PanDirectionDelegateCommand : DelegateCommand<PanDirection>
    {
        public PanDirectionDelegateCommand(Action<PanDirection> execute, Func<PanDirection, bool> canExecute = null) 
            : base(execute, canExecute)
        {
        }
    }
}
