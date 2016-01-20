using System.Windows;
using System.Windows.Input;

namespace Codartis.SoftVis.UI.Wpf.RoutedEvents
{
    /// <summary>
    /// MouseEventArgs wrapped for usage as routed event argument.
    /// </summary>
    public class BubblingMouseRoutedEventArgs : RoutedEventArgs
    {
        public MouseEventArgs MouseEventArgs { get; }

        public BubblingMouseRoutedEventArgs(RoutedEvent routedEvent, MouseEventArgs mouseEventArgs) 
            : base(routedEvent)
        {
            MouseEventArgs = mouseEventArgs;
        }
    }
}
