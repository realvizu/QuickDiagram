using System;
using Codartis.SoftVis.UI.Common;

namespace Codartis.SoftVis.UI.Wpf.Commands
{
    /// <summary>
    /// A command that invokes a delegate with a PanDirection parameter.
    /// </summary>
    public class PanDirectionCommand : DelegateCommand<PanDirection>
    {
        public PanDirectionCommand(Action<PanDirection> execute, Func<PanDirection, bool> canExecute = null) 
            : base(execute, canExecute)
        {
        }
    }
}
