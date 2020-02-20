using System;
using System.Windows;
using JetBrains.Annotations;
using Microsoft.Xaml.Behaviors;

namespace Codartis.Util.UI.Wpf.Behaviors
{
    /// <summary>
    /// Exposes an ActualSize property that always reflects the attached FrameworkElement's actual size.
    /// </summary>
    /// <remarks>
    /// This is not working with normal binding because Size is read-only that cannot be bound to in WPF (not even OneWayToSource).
    /// </remarks>
    public sealed class ActualSizeProviderBehavior : Behavior<FrameworkElement>
    {
        [NotNull] private readonly Delegate _onSizeChangedDelegate;

        public static readonly DependencyProperty ActualSizeProperty =
            DependencyProperty.Register(
                "ActualSize",
                typeof(Size),
                typeof(ActualSizeProviderBehavior),
                new FrameworkPropertyMetadata(Size.Empty) { BindsTwoWayByDefault = true });

        public ActualSizeProviderBehavior()
        {
            _onSizeChangedDelegate = new SizeChangedEventHandler(OnSizeChanged);
        }

        public Size ActualSize
        {
            get { return (Size)GetValue(ActualSizeProperty); }
            set { SetValue(ActualSizeProperty, value); }
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.AddHandler(FrameworkElement.SizeChangedEvent, _onSizeChangedDelegate);
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            AssociatedObject.RemoveHandler(FrameworkElement.SizeChangedEvent, _onSizeChangedDelegate);
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs args)
        {
            ActualSize = args.NewSize;
        }
    }
}