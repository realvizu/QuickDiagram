using System;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using JetBrains.Annotations;
using Microsoft.Xaml.Behaviors;

namespace Codartis.Util.UI.Wpf.Behaviors
{
    /// <summary>
    /// Copies the mouse position into the given properties when a specified event occurs.
    /// </summary>
    /// <remarks>
    /// The event's handler must be of type DependencyPropertyChangedEventHandler.
    /// The mouse position is relative to the AssociatedObject's parent.
    /// </remarks>
    public class CopyMousePositionBehavior : Behavior<FrameworkElement>
    {
        private EventInfo _eventInfo;
        private Delegate _eventHandler;

        public static readonly DependencyProperty OnEventNameProperty =
            DependencyProperty.Register("OnEventName", typeof(string), typeof(CopyMousePositionBehavior));

        public static readonly DependencyProperty YPropertyProperty =
            DependencyProperty.Register("YProperty", typeof(DependencyProperty), typeof(CopyMousePositionBehavior));

        public static readonly DependencyProperty XPropertyProperty =
            DependencyProperty.Register("XProperty", typeof(DependencyProperty), typeof(CopyMousePositionBehavior));

        public string OnEventName
        {
            get { return (string)GetValue(OnEventNameProperty); }
            set { SetValue(OnEventNameProperty, value); }
        }

        public DependencyProperty YProperty
        {
            get { return (DependencyProperty)GetValue(YPropertyProperty); }
            set { SetValue(YPropertyProperty, value); }
        }

        public DependencyProperty XProperty
        {
            get { return (DependencyProperty)GetValue(XPropertyProperty); }
            set { SetValue(XPropertyProperty, value); }
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
            AssociatedObject.SetValue(XProperty, position.X);
            AssociatedObject.SetValue(YProperty, position.Y);
        }

        [NotNull]
        private static Delegate CreateEventHandlerDelegate(
            [NotNull] Type evenHandlerType,
            [NotNull] object target,
            [NotNull] DependencyPropertyChangedEventHandler handler)
        {
            return Delegate.CreateDelegate(evenHandlerType, target, handler.Method);
        }
    }
}