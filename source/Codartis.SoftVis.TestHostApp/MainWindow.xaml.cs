using System.Reflection;
using Autofac;
using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.Diagramming.Definition.Layout;
using Codartis.SoftVis.Diagramming.Implementation;
using Codartis.SoftVis.Diagramming.Implementation.Layout;
using Codartis.SoftVis.Diagramming.Implementation.Layout.DirectConnector;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.SoftVis.Modeling.Implementation;
using Codartis.SoftVis.Services;
using Codartis.SoftVis.Services.Plugins;
using Codartis.SoftVis.TestHostApp.Diagramming;
using Codartis.SoftVis.TestHostApp.UI;
using Codartis.SoftVis.UI;
using Codartis.SoftVis.UI.Wpf;
using Codartis.SoftVis.UI.Wpf.View;
using Codartis.SoftVis.UI.Wpf.ViewModel;
using Codartis.Util.UI;
using Codartis.Util.UI.Wpf.Resources;

namespace Codartis.SoftVis.TestHostApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private const string DiagramStylesXaml = "Resources/Styles.xaml";

        public MainWindow()
        {
            var container = CreateDependencyContainer();

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

        private static IContainer CreateDependencyContainer()
        {
            var builder = new ContainerBuilder();

            RegisterModelComponents(builder);
            RegisterDiagramComponents(builder);
            RegisterDiagramUiComponents(builder);
            RegisterDiagramPlugins(builder);

            builder.RegisterType<VisualizationService>().SingleInstance().As<IVisualizationService>();

            builder.RegisterType<MainWindowViewModel>();

            return builder.Build();
        }

        private static void RegisterModelComponents(ContainerBuilder builder)
        {
            builder.RegisterType<ModelService>().SingleInstance().As<IModelService>().As<IModelEventSource>();
            builder.RegisterType<TestRelatedNodeTypeProvider>().As<IRelatedNodeTypeProvider>();
        }

        private static void RegisterDiagramComponents(ContainerBuilder builder)
        {
            builder.RegisterType<DiagramService>().As<IDiagramService>();
            builder.RegisterType<TestConnectorTypeResolver>().As<IConnectorTypeResolver>();

            builder.RegisterType<DiagramLayoutAlgorithm>().As<IDiagramLayoutAlgorithm>()
                .WithParameter("childrenAreaPadding", 2);

            builder.RegisterType<TestLayoutPriorityProvider>().As<ILayoutPriorityProvider>();
            builder.RegisterType<LayoutAlgorithmSelectionStrategy>().As<ILayoutAlgorithmSelectionStrategy>();
            builder.RegisterType<DirectConnectorRoutingAlgorithm>().As<IConnectorRoutingAlgorithm>();
        }

        private static void RegisterDiagramUiComponents(ContainerBuilder builder)
        {
            builder.RegisterType<WpfDiagramUiService>().As<IDiagramUiService>();

            builder.RegisterType<DiagramViewModel>().As<IDiagramUi>();

            builder.RegisterType<DiagramViewportViewModel>().As<IDiagramViewportUi>()
                .WithParameter("minZoom", .2)
                .WithParameter("maxZoom", 5d)
                .WithParameter("initialZoom", 1d);

            builder.RegisterType<DiagramShapeViewModelFactory>().As<IDiagramShapeUiFactory>();
            builder.RegisterType<TestNodePayloadUiFactory>().As<IPayloadUiFactory>();

            builder.RegisterType<MiniButtonPanelViewModel>().As<IDecorationManager<IMiniButton, IDiagramShapeUi>>();

            var resourceDictionary = ResourceHelpers.GetResourceDictionary(DiagramStylesXaml, Assembly.GetExecutingAssembly());

            builder.RegisterType<DiagramControl>()
                .WithParameter("additionalResourceDictionary", resourceDictionary);
        }

        private static void RegisterDiagramPlugins(ContainerBuilder builder)
        {
            builder.RegisterType<AutoLayoutDiagramPlugin>().As<IDiagramPlugin>();
            builder.RegisterType<ConnectorHandlerDiagramPlugin>().As<IDiagramPlugin>();
            builder.RegisterType<ModelTrackingDiagramPlugin>().As<IDiagramPlugin>();
        }
    }
}