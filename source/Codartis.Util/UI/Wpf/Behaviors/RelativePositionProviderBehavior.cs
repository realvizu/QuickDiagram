using System;
using System.Windows;
using Microsoft.Xaml.Behaviors;

namespace Codartis.Util.UI.Wpf.Behaviors
{
    /// <summary>
    /// Exposes a RelativePosition property that always reflects the attached FrameworkElement's relative position to a given control.
    /// </summary>
    public sealed class RelativePositionProviderBehavior : Behavior<FrameworkElement>
    {
        private static readonly Point TopLeft = new Point(0, 0);

        public static readonly DependencyProperty RelativePositionProperty =
            DependencyProperty.Register(
                "RelativePosition",
                typeof(Point),
                typeof(RelativePositionProviderBehavior),
                new FrameworkPropertyMetadata(new Point(0, 0)) { BindsTwoWayByDefault = true });

        public static readonly DependencyProperty ReferenceUiElementProperty =
            DependencyProperty.Register(
                "ReferenceUiElement",
                typeof(UIElement),
                typeof(RelativePositionProviderBehavior));

        public Point RelativePosition
        {
            get { return (Point)GetValue(RelativePositionProperty); }
            set { SetValue(RelativePositionProperty, value); }
        }

        public UIElement ReferenceUiElement
        {
            get { return (UIElement)GetValue(ReferenceUiElementProperty); }
            set { SetValue(ReferenceUiElementProperty, value); }
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            if (ReferenceUiElement == null)
                throw new Exception("Reference UI element not set.");

            ReferenceUiElement.LayoutUpdated += OnLayoutUpdated;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            if (ReferenceUiElement != null)
                ReferenceUiElement.LayoutUpdated -= OnLayoutUpdated;
        }

        private void OnLayoutUpdated(object sender, EventArgs args)
        {
            if (AssociatedObject != null)
                RelativePosition = AssociatedObject.TranslatePoint(TopLeft, ReferenceUiElement);
        }
    }
}