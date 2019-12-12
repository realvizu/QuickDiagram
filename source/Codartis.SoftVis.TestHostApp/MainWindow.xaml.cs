using Autofac;
using Codartis.SoftVis.Services;
using Codartis.SoftVis.UI.Wpf;

namespace Codartis.SoftVis.TestHostApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            var container = DependencyConfiguration.Create();

            var visualizationService = container.Resolve<IVisualizationService>();

            var modelService = visualizationService.GetModelService();
            var diagramId = visualizationService.CreateDiagram();
            var diagramService = visualizationService.GetDiagramService(diagramId);
            var wpfUiService = (IWpfDiagramUiService)visualizationService.GetDiagramUiService(diagramId);
            var mainWindowViewModel = new MainWindowViewModel(this, modelService, diagramService, wpfUiService);

            InitializeComponent();

            DataContext = mainWindowViewModel;
            DiagramControlPresenter.Content = wpfUiService.DiagramControl;
        }
    }
}