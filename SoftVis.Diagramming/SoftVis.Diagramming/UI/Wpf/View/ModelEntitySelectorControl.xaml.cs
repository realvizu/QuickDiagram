using System.Windows;
using System.Windows.Controls;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.UI.Common;
using Codartis.SoftVis.UI.Wpf.Commands;

namespace Codartis.SoftVis.UI.Wpf.View
{
    /// <summary>
    /// Interaction logic for ModelEntitySelectorControl.xaml
    /// </summary>
    public partial class ModelEntitySelectorControl : UserControl
    {
        private const HandleOrientation DefaultHandleOrientation = HandleOrientation.Bottom;

        public static readonly DependencyProperty HandleOrientationProperty =
            DependencyProperty.Register("HandleOrientation", typeof(HandleOrientation), typeof(ModelEntitySelectorControl),
                new FrameworkPropertyMetadata(HandleOrientation.None, FrameworkPropertyMetadataOptions.None, 
                    HandleOrientationProperty_Changed));

        private static void HandleOrientationProperty_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e) 
            => ((ModelEntitySelectorControl)d).SetHandleOrientation((HandleOrientation)e.NewValue);

        public static readonly DependencyProperty ModelEntitySelectedCommandProperty =
            DependencyProperty.Register("ModelEntitySelectedCommand", typeof(ModelEntityDelegateCommand), typeof(ModelEntitySelectorControl));

        public ModelEntitySelectorControl()
        {
            InitializeComponent();
            SetHandleOrientation(DefaultHandleOrientation);
        }

        public HandleOrientation HandleOrientation
        {
            get { return (HandleOrientation)GetValue(HandleOrientationProperty); }
            set { SetValue(HandleOrientationProperty, value); }
        }

        public ModelEntityDelegateCommand ModelEntitySelectedCommand
        {
            get { return (ModelEntityDelegateCommand)GetValue(ModelEntitySelectedCommandProperty); }
            set { SetValue(ModelEntitySelectedCommandProperty, value); }
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
            ModelEntitySelectedCommand.Execute(ListBox.SelectedItem as IModelEntity);
        }
    }
}
