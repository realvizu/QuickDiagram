using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Codartis.SoftVis.UI.Wpf.RoutedEvents;

namespace Codartis.SoftVis.UI.Wpf.View
{
    /// <summary>
    /// A special button used on diagrams. 
    /// Fires bubbling mouse enter and leave events.
    /// </summary>
    public class MiniButton : Button
    {
        public static readonly RoutedEvent BubblingMouseEnterEvent = 
            EventManager.RegisterRoutedEvent("BubblingMouseEnter", RoutingStrategy.Bubble, 
                typeof(BubblingMouseRoutedEventHandler), typeof(MiniButton));

        public static readonly RoutedEvent BubblingMouseLeaveEvent =
            EventManager.RegisterRoutedEvent("BubblingMouseLeave", RoutingStrategy.Bubble,
                typeof(BubblingMouseRoutedEventHandler), typeof(MiniButton));

        public MiniButton()
        {
            IsHitTestVisible = true;
        }

        public event BubblingMouseRoutedEventHandler BubblingMouseEnter
        {
            add { AddHandler(BubblingMouseEnterEvent, value); }
            remove { RemoveHandler(BubblingMouseEnterEvent, value); }
        }

        public event BubblingMouseRoutedEventHandler BubblingMouseLeave
        {
            add { AddHandler(BubblingMouseLeaveEvent, value); }
            remove { RemoveHandler(BubblingMouseLeaveEvent, value); }
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            RaiseEvent(new BubblingMouseRoutedEventArgs(BubblingMouseEnterEvent, e));
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            RaiseEvent(new BubblingMouseRoutedEventArgs(BubblingMouseLeaveEvent, e));
        }

        protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
        {
            return new PointHitTestResult(this, hitTestParameters.HitPoint);
        }
    }
}
