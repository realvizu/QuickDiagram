using System.Windows;

namespace Codartis.Util.UI.Wpf.Controls
{
    /// <summary>
    /// Interaction logic for ProgressWindow.xaml
    /// </summary>
    public partial class ProgressWindow : Window
    {
        public ProgressWindow()
        {
            InitializeComponent();
        }

        private void Cancel_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
