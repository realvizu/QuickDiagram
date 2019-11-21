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
using Codartis.Util.UI.Wpf.Resources;

namespace Codartis.SoftVis.VisualStudioIntegration.Hosting
{
    public static class DependencyConfigurator
    {
        private const string DiagramStylesXaml = "UI/DiagramStyles.xaml";

        public static IContainer CreateDependencyContainer(IVisualStudioServices visualStudioServices)
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<ModelService>().SingleInstance().As<IModelService>();
            builder.RegisterType<RelatedNodeTypeProvider>().As<IRelatedNodeTypeProvider>();

            builder.RegisterType<DiagramService>().As<IDiagramService>();
            builder.RegisterType<RoslynConnectorTypeResolver>().As<IConnectorTypeResolver>();

            builder.RegisterType<RoslynDiagramShapeViewModelFactory>().As<IDiagramShapeUiFactory>()
                .WithParameter("isDescriptionVisible", true);

            builder.RegisterType<DiagramViewportViewModel>()
                .WithParameter("minZoom", .2)
                .WithParameter("maxZoom", 5d)
                .WithParameter("initialZoom", 1d);

            builder.RegisterType<RoslynDiagramViewModel>().As<DiagramViewModel>();

            var resourceDictionary = ResourceHelpers.GetResourceDictionary(DiagramStylesXaml, Assembly.GetExecutingAssembly());

            builder.RegisterType<DiagramControl>()
                .WithParameter("additionalResourceDictionary", resourceDictionary);

            builder.RegisterType<DiagramWindowService>().As<IDiagramUiService>();

            builder.RegisterType<DiagramLayoutAlgorithm>()
                .WithParameter("childrenAreaPadding", 2)
                .As<IDiagramLayoutAlgorithm>();

            builder.RegisterType<LayoutPriorityProvider>().As<ILayoutPriorityProvider>();
            builder.RegisterType<LayoutAlgorithmSelectionStrategy>().As<ILayoutAlgorithmSelectionStrategy>();
            builder.RegisterType<DirectConnectorRoutingAlgorithm>().As<IConnectorRoutingAlgorithm>();

            builder.RegisterType<AutoLayoutDiagramPlugin>().As<IDiagramPlugin>();
            builder.RegisterType<ConnectorHandlerDiagramPlugin>().As<IDiagramPlugin>();
            builder.RegisterType<ModelTrackingDiagramPlugin>().As<IDiagramPlugin>();
            //builder.RegisterType<ModelExtenderDiagramPlugin>().As<IDiagramPlugin>();

            builder.RegisterType<VisualizationService>().SingleInstance().As<IVisualizationService>();

            builder.RegisterType<RoslynModelService>().As<IRoslynModelService>();

            var softVisPackage = new TypedParameter(typeof(IVisualStudioServices), visualStudioServices);
            builder.RegisterType<HostWorkspaceGateway>().WithParameter(softVisPackage).As<IHostModelProvider>();
            builder.RegisterType<HostUiGateway>().WithParameter(softVisPackage).As<IHostUiService>();

            builder.RegisterInstance(visualStudioServices).As<IVisualStudioServices>();

            builder.RegisterType<DiagramToolApplication>();

            return builder.Build();
        }
    }
}