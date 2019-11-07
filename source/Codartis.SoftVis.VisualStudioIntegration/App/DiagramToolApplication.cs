using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.SoftVis.Services;
using Codartis.SoftVis.VisualStudioIntegration.Modeling;
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

        public IModelService ModelService => _visualizationService.GetModelService();
        public IDiagramService DiagramService => _visualizationService.GetDiagramService(_diagramId);
        public IApplicationUiService ApplicationUiService => (IApplicationUiService)_visualizationService.GetDiagramUiService(_diagramId);

        public IRoslynModelService RoslynModelService { get; }

        public DiagramToolApplication(
            IHostUiServices hostUiServices,
            IVisualizationService visualizationService,
            IRoslynModelService roslynModelService)
        {
            _hostUiServices = hostUiServices;
            _visualizationService = visualizationService;
            
            RoslynModelService = roslynModelService;
            RoslynModelService.HideTrivialBaseNodes = AppDefaults.HideTrivialBaseNodes;

            _diagramId = _visualizationService.CreateDiagram();

            ApplicationUiService.ImageExportDpi = Dpi.Dpi150;
            ApplicationUiService.DiagramNodeInvoked += OnShowSourceRequested;
            ApplicationUiService.ShowModelItemsRequested += OnShowItemsRequested;
        }

        public void Run(Func<Task> asyncMethod) => _hostUiServices.Run(asyncMethod);

        private void OnShowSourceRequested(IDiagramShape diagramShape)
        {
            _hostUiServices.Run(async () => await OnShowSourceRequestAsync(diagramShape));
        }

        private Task OnShowSourceRequestAsync(IDiagramShape diagramShape)
        {
            var diagramNode = diagramShape as IDiagramNode;
            if (diagramNode == null)
                return Task.CompletedTask;

            //await new ShowSourceFileCommand(this, diagramNode).ExecuteAsync();
            return Task.CompletedTask;
        }

        private void OnShowItemsRequested(IReadOnlyList<IModelNode> modelNodes, bool followWithViewport)
        {
            _hostUiServices.Run(async () => await OnShowItemsRequestAsync(modelNodes, followWithViewport));
        }

        private Task OnShowItemsRequestAsync(IReadOnlyList<IModelNode> modelNodes, bool followWithViewport)
        {
            //var roslynModelNodes = modelNodes.OfType<IRoslynSymbol>().ToArray();
            //if (roslynModelNodes.Any())
            //    await new AddItemsToDiagramCommand(this, roslynModelNodes, followWithViewport).ExecuteAsync();
            return Task.CompletedTask;
        }
    }
}