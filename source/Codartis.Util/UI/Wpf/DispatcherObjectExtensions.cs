using System;
using System.Windows.Threading;

namespace Codartis.Util.UI.Wpf
{
    public static class DispatcherObjectExtensions
    {
        public static void EnsureThatDelayedRenderingOperationsAreCompleted(this DispatcherObject dispatcherObject)
        {
            dispatcherObject.Dispatcher.Invoke(DispatcherPriority.Loaded, new Action(() => { }));
        }
    }
}
