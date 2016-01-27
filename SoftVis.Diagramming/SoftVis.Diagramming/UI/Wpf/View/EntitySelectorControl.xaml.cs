using System.Windows;
using System.Windows.Controls;
using Codartis.SoftVis.UI.Common;

namespace Codartis.SoftVis.UI.Wpf.View
{
    /// <summary>
    /// Interaction logic for EntitySelectorControl.xaml
    /// </summary>
    public partial class EntitySelectorControl : UserControl
    {
        private const HandleOrientation DefaultHandleOrientation = HandleOrientation.Bottom;

        public static readonly DependencyProperty HandleOrientationProperty =
            DependencyProperty.Register("HandleOrientation", typeof(HandleOrientation), typeof(EntitySelectorControl),
                new FrameworkPropertyMetadata(HandleOrientation.None, FrameworkPropertyMetadataOptions.None, 
                    HandleOrientationProperty_Changed));

        private static void HandleOrientationProperty_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((EntitySelectorControl)d).SetHandleOrientation((HandleOrientation)e.NewValue);
        }

        public EntitySelectorControl()
        {
            InitializeComponent();
            SetHandleOrientation(DefaultHandleOrientation);
        }

        public HandleOrientation HandleOrientation
        {
            get { return (HandleOrientation)GetValue(HandleOrientationProperty); }
            set { SetValue(HandleOrientationProperty, value); }
        }

        private void SetHandleOrientation(HandleOrientation handleOrientation)
        {
            TopHandle.Visibility = BoolToVisibility(handleOrientation == HandleOrientation.Top);
            BottomHandle.Visibility = BoolToVisibility(handleOrientation == HandleOrientation.Bottom);
        }

        private static Visibility BoolToVisibility(bool value)
        {
            return value ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
