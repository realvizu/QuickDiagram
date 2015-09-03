using System.Windows;

namespace Codartis.SoftVis.TestHostApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var model = TestData.TestModel.Create();
            var diagram = TestData.TestDiagram.Create(model);
            DiagramViewerControl.DataContext = diagram;
        }
    }
}
