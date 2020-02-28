using System;
using System.Windows;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;

namespace Codartis.Util.UI.Wpf.MouseTracking
{
    public class MouseTrackingParticipantBehavior : Behavior<UIElement>
    {
        public static readonly DependencyProperty MouseTrackerProperty = DependencyProperty.Register(
            "MouseTracker", typeof(MouseTrackerBehavior), typeof(MouseTrackingParticipantBehavior),
            new PropertyMetadata(default(MouseTrackerBehavior)));

        public static readonly DependencyProperty RoleProperty = DependencyProperty.Register(
            "Role", typeof(MouseTrackingParticipantRole), typeof(MouseTrackingParticipantBehavior),
            new PropertyMetadata(MouseTrackingParticipantRole.TrackedElement));

        public MouseTrackerBehavior MouseTracker
        {
            get { return (MouseTrackerBehavior) GetValue(MouseTrackerProperty); }
            set { SetValue(MouseTrackerProperty, value); }
        }

        public MouseTrackingParticipantRole Role
        {
            get { return (MouseTrackingParticipantRole) GetValue(RoleProperty); }
            set { SetValue(RoleProperty, value); }
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            if (MouseTracker == null)
                throw new Exception("MouseTracker must be set.");

            switch (Role)
            {
                case MouseTrackingParticipantRole.TrackedElement:
                    AssociatedObject.MouseMove += OnMouseMoveOnTrackedElement;
                    break;
                case MouseTrackingParticipantRole.NonTrackedElement:
                    AssociatedObject.MouseMove += OnMouseMoveOnNonTrackedElement;
                    break;
            }
        }


        protected override void OnDetaching()
        {
            base.OnDetaching();

            switch (Role)
            {
                case MouseTrackingParticipantRole.TrackedElement:
                    AssociatedObject.MouseMove -= OnMouseMoveOnTrackedElement;
                    break;
                case MouseTrackingParticipantRole.NonTrackedElement:
                    AssociatedObject.MouseMove -= OnMouseMoveOnNonTrackedElement;
                    break;
            }
        }

        private void OnMouseMoveOnTrackedElement(object sender, MouseEventArgs e)
        {
            MouseTracker?.SetCurrent(sender as UIElement);
            e.Handled = true;
        }

        private void OnMouseMoveOnNonTrackedElement(object sender, MouseEventArgs e)
        {
            e.Handled = true;
        }
    }
}