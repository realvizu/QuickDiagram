namespace Codartis.SoftVis.TestHostApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            var viewModel = new MainWindowViewModel();
            DataContext = viewModel;

            InitializeComponent();

            viewModel.Window = this;
            viewModel.DiagramStlyeProvider = DiagramControl;
        }
    }
}

