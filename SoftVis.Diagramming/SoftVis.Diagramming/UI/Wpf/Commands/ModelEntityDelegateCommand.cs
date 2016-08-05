using System;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.Util.UI.Wpf;

namespace Codartis.SoftVis.UI.Wpf.Commands
{
    /// <summary>
    /// A command that invokes a delegate with an IModelEntity parameter.
    /// </summary>
    public class ModelEntityDelegateCommand : DelegateCommand<IModelEntity>
    {
        public ModelEntityDelegateCommand(Action<IModelEntity> execute, Func<IModelEntity, bool> canExecute = null) 
            : base(execute, canExecute)
        { }
    }
}
