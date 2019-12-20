using System.Collections.Generic;
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
using Codartis.Util.Ids;
using Codartis.Util.UI;
using Codartis.Util.UI.Wpf.Resources;
using Microsoft.CodeAnalysis;

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

            builder.RegisterType<VisualizationService>().As<IVisualizationService>().SingleInstance();
            builder.RegisterType<DiagramToolApplication>().SingleInstance();

            return builder.Build();
        }

        private static void RegisterModelComponents(ContainerBuilder builder)
        {
            builder.RegisterType<SequenceGenerator>().As<ISequenceProvider>().SingleInstance();
            builder.RegisterType<ModelRuleProvider>().As<IModelRuleProvider>().SingleInstance();

            builder.RegisterType<SymbolEqualityComparer>()
                .As<IEqualityComparer<ISymbol>>()
                .Named<IEqualityComparer<object>>("node")
                .SingleInstance();

            builder.Register(
                    i => new ModelService(
                        i.Resolve<ISequenceProvider>(),
                        i.ResolveOptional<IEnumerable<IModelRuleProvider>>(),
                        i.ResolveOptionalNamed<IEqualityComparer<object>>("node"),
                        i.ResolveOptionalNamed<IEqualityComparer<object>>("relationship")
                    ))
                .As<IModelService>()
                .As<IModelEventSource>()
                .SingleInstance();

            builder.RegisterType<RelatedNodeTypeProvider>().As<IRelatedNodeTypeProvider>().SingleInstance();
            builder.RegisterType<RelatedSymbolProvider>().As<IRelatedSymbolProvider>().SingleInstance();
            builder.RegisterType<RoslynBasedModelService>().As<IRoslynBasedModelService>().SingleInstance();
        }

        private static void RegisterDiagramComponents(ContainerBuilder builder)
        {
            builder.RegisterType<DiagramService>().As<IDiagramService>().SingleInstance();
            builder.RegisterType<RoslynConnectorTypeResolver>().As<IConnectorTypeResolver>().SingleInstance();

            builder.RegisterType<DiagramLayoutAlgorithm>().As<IDiagramLayoutAlgorithm>()
                .WithParameter("childrenAreaPadding", 2)
                .SingleInstance();

            builder.RegisterType<LayoutPriorityProvider>().As<ILayoutPriorityProvider>().SingleInstance();
            builder.RegisterType<LayoutAlgorithmSelectionStrategy>().As<ILayoutAlgorithmSelectionStrategy>().SingleInstance();
            builder.RegisterType<DirectConnectorRoutingAlgorithm>().As<IConnectorRoutingAlgorithm>().SingleInstance();
        }

        private static void RegisterDiagramUiComponents(ContainerBuilder builder)
        {
            builder.RegisterType<DiagramWindowService>().As<IDiagramUiService>().SingleInstance();

            builder.RegisterType<RoslynDiagramViewModel>().As<IDiagramUi>().SingleInstance();

            builder.RegisterType<RoslynDiagramViewportViewModel>().As<IDiagramViewportUi>()
                .WithParameter("minZoom", AppDefaults.MinZoom)
                .WithParameter("maxZoom", AppDefaults.MaxZoom)
                .WithParameter("initialZoom", AppDefaults.InitialZoom)
                .SingleInstance();

            builder.RegisterType<RoslynDiagramShapeViewModelFactory>().As<IDiagramShapeUiFactory>()
                .WithParameter("isDescriptionVisible", true)
                .SingleInstance();

            builder.RegisterType<MiniButtonPanelViewModel>().As<IDecorationManager<IMiniButton, IDiagramShapeUi>>().SingleInstance();

            var resourceDictionary = ResourceHelpers.GetResourceDictionary(DiagramStylesXaml, Assembly.GetExecutingAssembly());

            builder.RegisterType<DiagramControl>()
                .WithParameter("additionalResourceDictionary", resourceDictionary)
                .SingleInstance();

            builder.RegisterType<DataCloningDiagramImageCreator>().As<IDiagramImageCreator>().SingleInstance();
        }

        private static void RegisterDiagramPlugins(ContainerBuilder builder)
        {
            builder.RegisterType<AutoLayoutDiagramPlugin>().As<IDiagramPlugin>().SingleInstance();
            builder.RegisterType<ConnectorHandlerDiagramPlugin>().As<IDiagramPlugin>().SingleInstance();
            builder.RegisterType<ModelTrackingDiagramPlugin>().As<IDiagramPlugin>().SingleInstance();
            builder.RegisterType<ModelExtenderDiagramPlugin>().As<IDiagramPlugin>().SingleInstance();
        }

        private static void RegisterHostComponents(ContainerBuilder builder, IVisualStudioServices visualStudioServices)
        {
            var softVisPackage = new TypedParameter(typeof(IVisualStudioServices), visualStudioServices);
            builder.RegisterType<HostWorkspaceGateway>().WithParameter(softVisPackage).As<IRoslynWorkspaceProvider>().SingleInstance();
            builder.RegisterType<HostUiGateway>().WithParameter(softVisPackage).As<IHostUiService>().SingleInstance();

            builder.RegisterInstance(visualStudioServices).As<IVisualStudioServices>();
        }
    }
}