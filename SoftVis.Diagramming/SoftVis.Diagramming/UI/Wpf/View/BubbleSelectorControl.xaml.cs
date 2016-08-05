using System.Windows;
using System.Windows.Controls;
using Codartis.SoftVis.Util.UI.Wpf;

namespace Codartis.SoftVis.UI.Wpf.View
{
    /// <summary>
    /// Interaction logic for BubbleSelectorControl.xaml
    /// </summary>
    public partial class BubbleSelectorControl : UserControl
    {
        private const HandleOrientation DefaultHandleOrientation = HandleOrientation.Bottom;

        public static readonly DependencyProperty HandleOrientationProperty =
            DependencyProperty.Register("HandleOrientation", typeof(HandleOrientation), typeof(BubbleSelectorControl),
                new FrameworkPropertyMetadata(HandleOrientation.None, FrameworkPropertyMetadataOptions.None, 
                    HandleOrientationProperty_Changed));

        private static void HandleOrientationProperty_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e) 
            => ((BubbleSelectorControl)d).SetHandleOrientation((HandleOrientation)e.NewValue);

        public static readonly DependencyProperty ItemSelectedCommandProperty =
            DependencyProperty.Register("ItemSelectedCommand", typeof(DelegateCommand<object>), typeof(BubbleSelectorControl));

        public BubbleSelectorControl()
        {
            InitializeComponent();
            SetHandleOrientation(DefaultHandleOrientation);
        }

        public HandleOrientation HandleOrientation
        {
            get { return (HandleOrientation)GetValue(HandleOrientationProperty); }
            set { SetValue(HandleOrientationProperty, value); }
        }

        public DelegateCommand<object> ItemSelectedCommand
        {
            get { return (DelegateCommand<object>)GetValue(ItemSelectedCommandProperty); }
            set { SetValue(ItemSelectedCommandProperty, value); }
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

        private void ListBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ItemSelectedCommand.Execute(ListBox.SelectedItem);
        }
    }
}
