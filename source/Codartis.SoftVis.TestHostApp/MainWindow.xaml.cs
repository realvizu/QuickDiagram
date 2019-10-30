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
using Codartis.SoftVis.UI.Wpf.ViewModel;

namespace Codartis.SoftVis.TestHostApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            var container = CreateDependencyContainer();

            var viewModel = container.Resolve<MainWindowViewModel>();
            DataContext = viewModel;

            InitializeComponent();

            viewModel.OnUiInitialized(mainWindow: this, diagramStyleProvider: DiagramControl);
        }

        private static IContainer CreateDependencyContainer()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<MainWindowViewModel>();

            builder.RegisterType<ModelService>().SingleInstance().As<IModelService>();
            builder.RegisterType<TestRelatedNodeTypeProvider>().As<IRelatedNodeTypeProvider>();

            builder.RegisterType<DiagramService>().As<IDiagramService>();
            builder.RegisterType<TestConnectorTypeResolver>().As<IConnectorTypeResolver>();

            builder.RegisterType<DiagramShapeUiFactory>().As<IDiagramShapeUiFactory>();

            builder.RegisterType<WpfUiService>()
                .WithParameter("minZoom", .2)
                .WithParameter("maxZoom", 5d)
                .WithParameter("initialZoom", 1d)
                .As<IUiService>();

            builder.RegisterType<DiagramLayoutAlgorithm>()
                .WithParameter("childrenAreaPadding", 2)
                .As<IDiagramLayoutAlgorithm>();

            builder.RegisterType<TestLayoutPriorityProvider>().As<ILayoutPriorityProvider>();
            builder.RegisterType<LayoutAlgorithmSelectionStrategy>().As<ILayoutAlgorithmSelectionStrategy>();
            builder.RegisterType<DirectConnectorRoutingAlgorithm>().As<IConnectorRoutingAlgorithm>();

            builder.RegisterType<AutoLayoutDiagramPlugin>().As<IDiagramPlugin>();
            builder.RegisterType<ConnectorHandlerDiagramPlugin>().As<IDiagramPlugin>();
            builder.RegisterType<ModelTrackingDiagramPlugin>().As<IDiagramPlugin>();

            builder.RegisterType<VisualizationService>().SingleInstance().As<IVisualizationService>();

            return builder.Build();
        }
    }
}