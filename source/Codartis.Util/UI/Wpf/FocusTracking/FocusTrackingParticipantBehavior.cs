using System;
using System.Windows;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;

namespace Codartis.Util.UI.Wpf.FocusTracking
{
    public class FocusTrackingParticipantBehavior : Behavior<UIElement>
    {
        public static readonly DependencyProperty FocusTrackerProperty = DependencyProperty.Register(
            "FocusTracker", typeof(FocusTrackerBehavior), typeof(FocusTrackingParticipantBehavior),
            new PropertyMetadata(default(FocusTrackerBehavior)));

        public static readonly DependencyProperty RoleProperty = DependencyProperty.Register(
            "Role", typeof(FocusTrackingParticipantRole), typeof(FocusTrackingParticipantBehavior),
            new PropertyMetadata(FocusTrackingParticipantRole.TrackedElement));

        public FocusTrackerBehavior FocusTracker
        {
            get { return (FocusTrackerBehavior) GetValue(FocusTrackerProperty); }
            set { SetValue(FocusTrackerProperty, value); }
        }

        public FocusTrackingParticipantRole Role
        {
            get { return (FocusTrackingParticipantRole) GetValue(RoleProperty); }
            set { SetValue(RoleProperty, value); }
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            if (FocusTracker == null)
                throw new Exception("FocusTracker must be set.");

            switch (Role)
            {
                case FocusTrackingParticipantRole.TrackedElement:
                    AssociatedObject.MouseMove += OnMouseMoveOnTrackedElement;
                    break;
                case FocusTrackingParticipantRole.NonTrackedElement:
                    AssociatedObject.MouseMove += OnMouseMoveOnNonTrackedElement;
                    break;
            }
        }


        protected override void OnDetaching()
        {
            base.OnDetaching();

            switch (Role)
            {
                case FocusTrackingParticipantRole.TrackedElement:
                    AssociatedObject.MouseMove -= OnMouseMoveOnTrackedElement;
                    break;
                case FocusTrackingParticipantRole.NonTrackedElement:
                    AssociatedObject.MouseMove -= OnMouseMoveOnNonTrackedElement;
                    break;
            }
        }

        private void OnMouseMoveOnTrackedElement(object sender, MouseEventArgs e)
        {
            FocusTracker?.Focus(sender as UIElement);
            e.Handled = true;
        }

        private void OnMouseMoveOnNonTrackedElement(object sender, MouseEventArgs e)
        {
            e.Handled = true;
        }
    }
}