using System;
using System.Windows.Media.Imaging;

namespace Codartis.SoftVis.UI.Wpf.Commands
{
    /// <summary>
    /// A command that invokes a delegate with a BitmapSource parameter.
    /// </summary>
    public class BitmapSourceDelegateCommand : DelegateCommand<BitmapSource>
    {
        public BitmapSourceDelegateCommand(Action<BitmapSource> execute, Func<BitmapSource, bool> canExecute = null)
            : base(execute, canExecute)
        {
        }
    }
}
