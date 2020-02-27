using System;
using System.Windows;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;

namespace Codartis.Util.UI.Wpf.FocusTracking
{
    public sealed class FocusTrackerBehavior : Behavior<UIElement>
    {
        public static readonly DependencyProperty BackgroundElementProperty = DependencyProperty.Register(
            "BackgroundElement", typeof(UIElement), typeof(FocusTrackerBehavior),
            new PropertyMetadata(default(UIElement)));

        public static readonly DependencyProperty FocusedElementProperty = DependencyProperty.Register(
            "FocusedElement", typeof(UIElement), typeof(FocusTrackerBehavior),
            new PropertyMetadata(default(UIElement)));

        public UIElement BackgroundElement
        {
            get { return (UIElement) GetValue(BackgroundElementProperty); }
            set { SetValue(BackgroundElementProperty, value); }
        }

        public UIElement FocusedElement
        {
            get { return (UIElement) GetValue(FocusedElementProperty); }
            set { SetValue(FocusedElementProperty, value); }
        }

        public void Focus(UIElement element)
        {
            FocusedElement = element;
        }

        public void RemoveFocus()
        {
            FocusedElement = null;
        }

        protected override void OnAttached()
        {
            if (BackgroundElement == null)
                throw new Exception("BackgroundElement must be set.");

            BackgroundElement.MouseMove += OnMouseMoveOnBackground;
            BackgroundElement.MouseLeave += OnMouseLeaveOnBackground;
        }

        protected override void OnDetaching()
        {
            BackgroundElement.MouseMove -= OnMouseMoveOnBackground;
            BackgroundElement.MouseLeave -= OnMouseLeaveOnBackground;
        }

        private void OnMouseMoveOnBackground(object sender, MouseEventArgs e) => RemoveFocus();

        private void OnMouseLeaveOnBackground(object sender, MouseEventArgs e) => RemoveFocus();
    }
}