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
using Codartis.SoftVis.VisualStudioIntegration.Plugins;
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
            builder.RegisterType<DiagramToolApplication>().SingleInstance();

            return builder.Build();
        }

        private static void RegisterModelComponents(ContainerBuilder builder)
        {
            var payloadComparer = new RoslynSymbolEqualityComparer();
            builder.RegisterType<ModelService>()
                .WithParameter("payloadEqualityComparer", payloadComparer)
                .As<IModelService>()
                .As<IModelEventSource>()
                .SingleInstance();

            builder.RegisterType<RelatedNodeTypeProvider>().SingleInstance().As<IRelatedNodeTypeProvider>();
            builder.RegisterType<RelatedSymbolProvider>().SingleInstance().As<IRelatedSymbolProvider>();
            builder.RegisterType<RoslynModelService>().SingleInstance().As<IRoslynModelService>();
        }

        private static void RegisterDiagramComponents(ContainerBuilder builder)
        {
            builder.RegisterType<DiagramService>().SingleInstance().As<IDiagramService>();
            builder.RegisterType<RoslynConnectorTypeResolver>().SingleInstance().As<IConnectorTypeResolver>();

            builder.RegisterType<DiagramLayoutAlgorithm>().SingleInstance().As<IDiagramLayoutAlgorithm>()
                .WithParameter("childrenAreaPadding", 2);

            builder.RegisterType<LayoutPriorityProvider>().SingleInstance().As<ILayoutPriorityProvider>();
            builder.RegisterType<LayoutAlgorithmSelectionStrategy>().SingleInstance().As<ILayoutAlgorithmSelectionStrategy>();
            builder.RegisterType<DirectConnectorRoutingAlgorithm>().SingleInstance().As<IConnectorRoutingAlgorithm>();
        }

        private static void RegisterDiagramUiComponents(ContainerBuilder builder)
        {
            builder.RegisterType<DiagramWindowService>().SingleInstance().As<IDiagramUiService>();

            builder.RegisterType<RoslynDiagramViewModel>().SingleInstance().As<IDiagramUi>();

            builder.RegisterType<RoslynDiagramViewportViewModel>().SingleInstance().As<IDiagramViewportUi>()
                .WithParameter("minZoom", .2)
                .WithParameter("maxZoom", 5d)
                .WithParameter("initialZoom", 1d);

            builder.RegisterType<RoslynDiagramShapeViewModelFactory>().SingleInstance().As<IDiagramShapeUiFactory>()
                .WithParameter("isDescriptionVisible", true);

            builder.RegisterType<MiniButtonPanelViewModel>().SingleInstance().As<IDecorationManager<IMiniButton, IDiagramShapeUi>>();

            var resourceDictionary = ResourceHelpers.GetResourceDictionary(DiagramStylesXaml, Assembly.GetExecutingAssembly());

            builder.RegisterType<DiagramControl>().SingleInstance()
                .WithParameter("additionalResourceDictionary", resourceDictionary);

            builder.RegisterType<DataCloningDiagramImageCreator>().SingleInstance().As<IDiagramImageCreator>();
        }

        private static void RegisterDiagramPlugins(ContainerBuilder builder)
        {
            builder.RegisterType<AutoLayoutDiagramPlugin>().SingleInstance().As<IDiagramPlugin>();
            builder.RegisterType<ConnectorHandlerDiagramPlugin>().SingleInstance().As<IDiagramPlugin>();
            builder.RegisterType<ModelTrackingDiagramPlugin>().SingleInstance().As<IDiagramPlugin>();
            builder.RegisterType<ModelExtenderDiagramPlugin>().SingleInstance().As<IDiagramPlugin>();
        }

        private static void RegisterHostComponents(ContainerBuilder builder, IVisualStudioServices visualStudioServices)
        {
            var softVisPackage = new TypedParameter(typeof(IVisualStudioServices), visualStudioServices);
            builder.RegisterType<HostWorkspaceGateway>().SingleInstance().WithParameter(softVisPackage).As<IHostModelProvider>();
            builder.RegisterType<HostUiGateway>().SingleInstance().WithParameter(softVisPackage).As<IHostUiService>();

            builder.RegisterInstance(visualStudioServices).As<IVisualStudioServices>();
        }
    }
}