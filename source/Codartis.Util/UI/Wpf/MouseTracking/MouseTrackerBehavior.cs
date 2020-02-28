using System;
using System.Windows;
using System.Windows.Input;
using Codartis.Util.UI.Wpf.Commands;
using Microsoft.Xaml.Behaviors;

namespace Codartis.Util.UI.Wpf.MouseTracking
{
    public sealed class MouseTrackerBehavior : Behavior<UIElement>
    {
        public static readonly DependencyProperty BackgroundElementProperty = DependencyProperty.Register(
            "BackgroundElement",
            typeof(UIElement),
            typeof(MouseTrackerBehavior),
            new PropertyMetadata(default(UIElement)));

        public static readonly DependencyProperty CurrentUiElementProperty = DependencyProperty.Register(
            "CurrentUiElement",
            typeof(UIElement),
            typeof(MouseTrackerBehavior),
            new PropertyMetadata(OnCurrentUiElementChanged));

        private static void OnCurrentUiElementChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((MouseTrackerBehavior)d).CurrentUiElementChangedCommand?.Execute(e.NewValue as UIElement);
        }

        public static readonly DependencyProperty CurrentUiElementChangedCommandProperty =
            DependencyProperty.Register(
                "CurrentUiElementChangedCommand",
                typeof(DelegateCommand<UIElement>),
                typeof(MouseTrackerBehavior));

        public UIElement BackgroundElement
        {
            get { return (UIElement)GetValue(BackgroundElementProperty); }
            set { SetValue(BackgroundElementProperty, value); }
        }

        public UIElement CurrentUiElement
        {
            get { return (UIElement)GetValue(CurrentUiElementProperty); }
            set { SetValue(CurrentUiElementProperty, value); }
        }

        public DelegateCommand<UIElement> CurrentUiElementChangedCommand
        {
            get { return (DelegateCommand<UIElement>)GetValue(CurrentUiElementChangedCommandProperty); }
            set { SetValue(CurrentUiElementChangedCommandProperty, value); }
        }

        public void SetCurrent(UIElement element)
        {
            CurrentUiElement = element;
        }

        public void ClearCurrent()
        {
            CurrentUiElement = null;
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

        private void OnMouseMoveOnBackground(object sender, MouseEventArgs e) => ClearCurrent();

        private void OnMouseLeaveOnBackground(object sender, MouseEventArgs e) => ClearCurrent();
    }
}