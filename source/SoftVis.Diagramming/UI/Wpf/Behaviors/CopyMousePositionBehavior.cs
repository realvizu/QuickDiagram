using System;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;

namespace Codartis.SoftVis.UI.Wpf.Behaviors
{
    /// <summary>
    /// Copies the mouse position into the given properties when a specified event occurs.
    /// </summary>
    /// <remarks>
    /// The event's handler must be of type DependencyPropertyChangedEventHandler.
    /// The mouse position is relative to the AssociatedObject's parent.
    /// </remarks>
    internal class CopyMousePositionBehavior : Behavior<FrameworkElement>
    {
        private EventInfo _eventInfo;
        private Delegate _eventHandler;

        public static readonly DependencyProperty OnEventNameProperty =
            DependencyProperty.Register("OnEventName", typeof(string), typeof(CopyMousePositionBehavior));

        public static readonly DependencyProperty TopPropertyProperty =
            DependencyProperty.Register("TopProperty", typeof(DependencyProperty), typeof(CopyMousePositionBehavior));

        public static readonly DependencyProperty LeftPropertyProperty =
            DependencyProperty.Register("LeftProperty", typeof(DependencyProperty), typeof(CopyMousePositionBehavior));

        public string OnEventName
        {
            get { return (string)GetValue(OnEventNameProperty); }
            set { SetValue(OnEventNameProperty, value); }
        }

        public DependencyProperty TopProperty
        {
            get { return (DependencyProperty)GetValue(TopPropertyProperty); }
            set { SetValue(TopPropertyProperty, value); }
        }

        public DependencyProperty LeftProperty
        {
            get { return (DependencyProperty)GetValue(LeftPropertyProperty); }
            set { SetValue(LeftPropertyProperty, value); }
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            _eventInfo = AssociatedObject.GetType().GetEvent(OnEventName);
            if (_eventInfo == null)
                throw new Exception($"No event '{OnEventName}' found.");

            if (_eventInfo.EventHandlerType != typeof(DependencyPropertyChangedEventHandler))
                throw new Exception("The event's handler must be of type DependencyPropertyChangedEventHandler.");

            _eventHandler = CreateEventHandlerDelegate(_eventInfo.EventHandlerType, this, (o, e) => CopyMousePosition());
            _eventInfo.AddEventHandler(AssociatedObject, _eventHandler);
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            _eventInfo?.RemoveEventHandler(AssociatedObject, _eventHandler);
        }

        private void CopyMousePosition()
        {
            var parentInputElement = AssociatedObject.Parent as IInputElement;
            if (parentInputElement == null)
                return;

            var position = Mouse.GetPosition(parentInputElement);
            AssociatedObject.SetValue(LeftProperty, position.X);
            AssociatedObject.SetValue(TopProperty, position.Y);
        }

        private static Delegate CreateEventHandlerDelegate(Type evenHandlerType, object target,
            DependencyPropertyChangedEventHandler handler)
        {
            return Delegate.CreateDelegate(evenHandlerType, target, handler.Method);
        }
    }
}
