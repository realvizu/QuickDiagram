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
using Codartis.SoftVis.UI;
using Codartis.SoftVis.UI.Wpf.View;
using Codartis.SoftVis.UI.Wpf.ViewModel;
using Codartis.SoftVis.VisualStudioIntegration.App;
using Codartis.SoftVis.VisualStudioIntegration.Diagramming;
using Codartis.SoftVis.VisualStudioIntegration.Modeling;
using Codartis.SoftVis.VisualStudioIntegration.Modeling.Implementation;
using Codartis.SoftVis.VisualStudioIntegration.UI;
using Codartis.Util.UI;
using Codartis.Util.UI.Wpf.Resources;

namespace Codartis.SoftVis.VisualStudioIntegration.Hosting
{
    public static class DependencyConfigurator
    {
        private const string DiagramStylesXaml = "UI/DiagramStyles.xaml";

        public static IContainer CreateDependencyContainer(IVisualStudioServices visualStudioServices)
        {
            var builder = new ContainerBuilder();

            RegisterModelComponents(builder);
            RegisterDiagramComponents(builder);
            RegisterDiagramUiComponents(builder);
            RegisterDiagramPlugins(builder);

            RegisterHostComponents(builder, visualStudioServices);

            builder.RegisterType<VisualizationService>().SingleInstance().As<IVisualizationService>();

            builder.RegisterType<DiagramToolApplication>();

            return builder.Build();
        }

        private static void RegisterModelComponents(ContainerBuilder builder)
        {
            builder.RegisterType<ModelService>().SingleInstance().As<IModelService>().As<IModelEventSource>();
            builder.RegisterType<RelatedNodeTypeProvider>().As<IRelatedNodeTypeProvider>();

            builder.RegisterType<RoslynModelService>().As<IRoslynModelService>();
        }

        private static void RegisterDiagramComponents(ContainerBuilder builder)
        {
            builder.RegisterType<DiagramService>().As<IDiagramService>();
            builder.RegisterType<RoslynConnectorTypeResolver>().As<IConnectorTypeResolver>();

            builder.RegisterType<DiagramLayoutAlgorithm>().As<IDiagramLayoutAlgorithm>()
                .WithParameter("childrenAreaPadding", 2);

            builder.RegisterType<LayoutPriorityProvider>().As<ILayoutPriorityProvider>();
            builder.RegisterType<LayoutAlgorithmSelectionStrategy>().As<ILayoutAlgorithmSelectionStrategy>();
            builder.RegisterType<DirectConnectorRoutingAlgorithm>().As<IConnectorRoutingAlgorithm>();
        }

        private static void RegisterDiagramUiComponents(ContainerBuilder builder)
        {
            builder.RegisterType<DiagramWindowService>().As<IDiagramUiService>();

            builder.RegisterType<RoslynDiagramViewModel>().As<IDiagramUi>();

            builder.RegisterType<RoslynDiagramViewportViewModel>().As<IDiagramViewportUi>()
                .WithParameter("minZoom", .2)
                .WithParameter("maxZoom", 5d)
                .WithParameter("initialZoom", 1d);

            builder.RegisterType<RoslynDiagramShapeViewModelFactory>().As<IDiagramShapeUiFactory>()
                .WithParameter("isDescriptionVisible", true);

            builder.RegisterType<MiniButtonPanelViewModel>().As<IDecorationManager<IMiniButton, IDiagramShapeUi>>();

            var resourceDictionary = ResourceHelpers.GetResourceDictionary(DiagramStylesXaml, Assembly.GetExecutingAssembly());

            builder.RegisterType<DiagramControl>()
                .WithParameter("additionalResourceDictionary", resourceDictionary);

            builder.RegisterType<DataCloningDiagramImageCreator>().As<IDiagramImageCreator>();
        }

        private static void RegisterDiagramPlugins(ContainerBuilder builder)
        {
            builder.RegisterType<AutoLayoutDiagramPlugin>().As<IDiagramPlugin>();
            builder.RegisterType<ConnectorHandlerDiagramPlugin>().As<IDiagramPlugin>();
            builder.RegisterType<ModelTrackingDiagramPlugin>().As<IDiagramPlugin>();
            //builder.RegisterType<ModelExtenderDiagramPlugin>().As<IDiagramPlugin>();
        }

        private static void RegisterHostComponents(ContainerBuilder builder, IVisualStudioServices visualStudioServices)
        {
            var softVisPackage = new TypedParameter(typeof(IVisualStudioServices), visualStudioServices);
            builder.RegisterType<HostWorkspaceGateway>().WithParameter(softVisPackage).As<IHostModelProvider>();
            builder.RegisterType<HostUiGateway>().WithParameter(softVisPackage).As<IHostUiService>();

            builder.RegisterInstance(visualStudioServices).As<IVisualStudioServices>();
        }
    }
}