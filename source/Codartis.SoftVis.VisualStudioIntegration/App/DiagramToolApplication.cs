using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Diagramming.Layout.Nodes.Vertical;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.Services;
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
        private readonly IHostUiServices _hostUiServices;
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
            _hostUiServices = hostUiServices;
            _visualizationService = CreateVisualizationService(roslynModelProvider, hostUiServices);
            _diagramId = _visualizationService.CreateDiagram(AppDefaults.MinZoom, AppDefaults.MaxZoom, AppDefaults.InitialZoom);

            RoslynModelService.HideTrivialBaseNodes = AppDefaults.HideTrivialBaseNodes;

            ApplicationUiService.ImageExportDpi = Dpi.Dpi150;
            ApplicationUiService.DiagramNodeInvoked += OnShowSourceRequested;
            ApplicationUiService.ShowModelItemsRequested += OnShowItemsRequested;
        }

        public void Run(Func<Task> asyncMethod) => _hostUiServices.Run(asyncMethod);

        private static IVisualizationService CreateVisualizationService(IRoslynModelProvider roslynModelProvider, IHostUiServices hostUiServices)
        {
            return new VisualizationService(
                new RoslynModelServiceFactory(roslynModelProvider),
                new RoslynDiagramServiceFactory(),
                new ApplicationUiServiceFactory(hostUiServices, AppDefaults.NodeDescriptionsVisibleByDefault),
                new ApplicationDiagramPluginFactory(new RoslynLayoutPriorityProvider(), new RoslynDiagramShapeFactory(), new VerticalNodeLayoutAlgorithm(),  hostUiServices),
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
            _hostUiServices.Run(async () => await OnShowSourceRequestAsync(diagramShape));
        }

        private async Task OnShowSourceRequestAsync(IDiagramShape diagramShape)
        {
            var diagramNode = diagramShape as IDiagramNode;
            if (diagramNode == null)
                return;

            await new ShowSourceFileCommand(this, diagramNode).ExecuteAsync();
        }

        private void OnShowItemsRequested(IReadOnlyList<IModelNode> modelNodes, bool followWithViewport)
        {
            _hostUiServices.Run(async () => await OnShowItemsRequestAsync(modelNodes, followWithViewport));
        }

        private async Task OnShowItemsRequestAsync(IReadOnlyList<IModelNode> modelNodes, bool followWithViewport)
        {
            var roslynModelNodes = modelNodes.OfType<IRoslynModelNode>().ToArray();
            if (roslynModelNodes.Any())
                await new AddItemsToDiagramCommand(this, roslynModelNodes, followWithViewport).ExecuteAsync();
        }
    }
}