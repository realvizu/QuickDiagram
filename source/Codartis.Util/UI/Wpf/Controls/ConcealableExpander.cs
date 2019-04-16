using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace Codartis.Util.UI.Wpf.Controls
{
    /// <summary>
    /// An expander whose button can be hidden.
    /// If the button is hidden then the user can't operate the control but it can still be operated programatically.
    /// </summary>
    public class ConcealableExpander : Expander
    {
        static ConcealableExpander()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ConcealableExpander),
                new FrameworkPropertyMetadata(typeof(ConcealableExpander)));
        }

        public static readonly DependencyProperty IsButtonVisibleProperty =
            DependencyProperty.Register("IsButtonVisible", typeof(bool), typeof(ConcealableExpander),
                new FrameworkPropertyMetadata(defaultValue: true));

        public bool IsButtonVisible
        {
            get { return (bool)GetValue(IsButtonVisibleProperty); }
            set { SetValue(IsButtonVisibleProperty, value); }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            BindExpanderToggleButtonVisibilityToIsButtonVisible();
        }

        private void BindExpanderToggleButtonVisibilityToIsButtonVisible()
        {
            var toggleButton = this.FindFirstDescendant<ToggleButton>();
            var binding = new Binding
            {
                Source = this,
                Path = new PropertyPath(IsButtonVisibleProperty),
                Converter = new BooleanToVisibilityConverter(),
            };
            toggleButton.SetBinding(VisibilityProperty, binding);
        }
    }
}
