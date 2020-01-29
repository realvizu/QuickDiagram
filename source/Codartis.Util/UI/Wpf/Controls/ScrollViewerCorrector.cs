using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Codartis.Util.UI.Wpf.Controls
{
    /// <summary>
    /// Implements an attached property that prevents ScrollViewers from swallowing the mouse wheel events
    /// even if they are not used for scrolling the ScrollViewer.
    /// </summary>
    /// <remarks>
    /// From:
    /// https://serialseb.com/blog/2007/09/03/wpf-tips-6-preventing-scrollviewer-from/
    /// </remarks>
    public sealed class ScrollViewerCorrector
    {
        private static readonly List<MouseWheelEventArgs> ReEntrantList = new List<MouseWheelEventArgs>();

        public static readonly DependencyProperty FixScrollingProperty =
            DependencyProperty.RegisterAttached(
                "FixScrolling",
                typeof(bool),
                typeof(ScrollViewerCorrector),
                new FrameworkPropertyMetadata(false, OnFixScrollingPropertyChanged));

        public static bool GetFixScrolling(DependencyObject obj)
        {
            return (bool)obj.GetValue(FixScrollingProperty);
        }

        public static void SetFixScrolling(DependencyObject obj, bool value)
        {
            obj.SetValue(FixScrollingProperty, value);
        }

        public static void OnFixScrollingPropertyChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is ScrollViewer viewer))
                throw new ArgumentException("The dependency property can only be attached to a ScrollViewer", nameof(sender));

            if ((bool)e.NewValue)
                viewer.PreviewMouseWheel += HandlePreviewMouseWheel;
            else
                viewer.PreviewMouseWheel -= HandlePreviewMouseWheel;
        }

        private static void HandlePreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var scrollControl = sender as ScrollViewer;

            if (e.Handled || sender == null || ReEntrantList.Contains(e))
                return;

            var previewEventArgs = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta)
            {
                RoutedEvent = UIElement.PreviewMouseWheelEvent,
                Source = sender
            };

            var originalSource = e.OriginalSource as UIElement;

            ReEntrantList.Add(previewEventArgs);

            originalSource.RaiseEvent(previewEventArgs);

            ReEntrantList.Remove(previewEventArgs);

            if (previewEventArgs.Handled || !ScrollingLimitReached(e, scrollControl))
                return;

            e.Handled = true;

            RaiseEventOnParent(sender, e);
        }

        private static void RaiseEventOnParent(object sender, MouseWheelEventArgs e)
        {
            var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta)
            {
                RoutedEvent = UIElement.MouseWheelEvent,
                Source = sender
            };

            var parent = (UIElement)((FrameworkElement)sender).Parent;

            parent.RaiseEvent(eventArg);
        }

        private static bool ScrollingLimitReached(MouseWheelEventArgs e, ScrollViewer scrollControl)
        {
            return e.Delta > 0 && scrollControl.VerticalOffset.IsEqualWithTolerance(0) ||
                   e.Delta <= 0 && scrollControl.VerticalOffset >= scrollControl.ExtentHeight - scrollControl.ViewportHeight;
        }
    }
}