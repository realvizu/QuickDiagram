using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.Service;
using Codartis.SoftVis.VisualStudioIntegration.App.Commands;
using Codartis.SoftVis.VisualStudioIntegration.Diagramming;
using Codartis.SoftVis.VisualStudioIntegration.Modeling;
using Codartis.SoftVis.VisualStudioIntegration.Modeling.Implementation;
using Codartis.SoftVis.VisualStudioIntegration.Plugins;
using Codartis.SoftVis.VisualStudioIntegration.UI;

namespace Codartis.SoftVis.VisualStudioIntegration.App
{
    /// <summary>
    /// The diagram tool application.
    /// </summary>
    /// <remarks>
    /// Sets up the model, the diagram, and the commands that implement the application logic.
    /// Provides application services to the commands.
    /// </remarks>
    internal sealed class DiagramToolApplication : IAppServices
    {
        private readonly IVisualizationService _visualizationService;
        private readonly DiagramId _diagramId;

        public IRoslynModelService RoslynModelService => (IRoslynModelService) _visualizationService.GetModelService();
        public IRoslynDiagramService RoslynDiagramService => (IRoslynDiagramService) _visualizationService.GetDiagramService(_diagramId);
        public IApplicationUiService ApplicationUiService => (IApplicationUiService) _visualizationService.GetUiService(_diagramId);

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public DiagramToolApplication(IRoslynModelProvider roslynModelProvider, IHostUiServices hostUiServices)
        {
            _visualizationService = CreateVisualizationService(roslynModelProvider, hostUiServices);
            _diagramId = _visualizationService.CreateDiagram(AppDefaults.MinZoom, AppDefaults.MaxZoom, AppDefaults.InitialZoom);

            RoslynModelService.HideTrivialBaseNodes = AppDefaults.HideTrivialBaseNodes;

            ApplicationUiService.ImageExportDpi = Dpi.Dpi150;
            ApplicationUiService.DiagramNodeInvoked += OnShowSourceRequested;
            ApplicationUiService.ShowModelItemsRequested += OnShowItemsRequestedAsync;
        }

        private static IVisualizationService CreateVisualizationService(IRoslynModelProvider roslynModelProvider, IHostUiServices hostUiServices)
        {
            return new VisualizationService(
                new RoslynModelServiceFactory(roslynModelProvider),
                new RoslynDiagramServiceFactory(),
                new ApplicationUiServiceFactory(hostUiServices, AppDefaults.NodeDescriptionsVisibleByDefault),
                new ApplicationDiagramPluginFactory(new RoslynLayoutPriorityProvider(), new RoslynDiagramShapeFactory()),
                new[]
                {
                    DiagramPluginId.AutoLayoutDiagramPlugin,
                    DiagramPluginId.ModelTrackingDiagramPlugin,
                    DiagramPluginId.ConnectorHandlerDiagramPlugin,
                    ApplicationDiagramPluginId.ModelExtenderDiagramPlugin
                });
        }

        private void OnShowSourceRequested(IDiagramShape diagramShape)
        {
            var diagramNode = diagramShape as IDiagramNode;
            if (diagramNode == null)
                return;

            new ShowSourceFileCommand(this).Execute(diagramNode);
        }

#pragma warning disable VSTHRD100 // Avoid async void methods
        private async void OnShowItemsRequestedAsync(IReadOnlyList<IModelNode> modelNodes, bool followWithViewport)
#pragma warning restore VSTHRD100 // Avoid async void methods
        {
            var roslynModelNodes = modelNodes.OfType<IRoslynModelNode>().ToArray();
            if (roslynModelNodes.Any())
                await new AddItemsToDiagramCommand(this).ExecuteAsync(roslynModelNodes, followWithViewport);
        }
    }
}