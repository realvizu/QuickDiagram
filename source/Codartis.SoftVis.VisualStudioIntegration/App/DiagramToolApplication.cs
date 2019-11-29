using System.Collections.Generic;
using System.Threading.Tasks;
using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.SoftVis.Services;
using Codartis.SoftVis.VisualStudioIntegration.App.Commands;
using Codartis.SoftVis.VisualStudioIntegration.Modeling;
using Codartis.SoftVis.VisualStudioIntegration.UI;
using JetBrains.Annotations;

namespace Codartis.SoftVis.VisualStudioIntegration.App
{
    /// <summary>
    /// The diagram tool application.
    /// </summary>
    /// <remarks>
    /// Sets up the model and the diagram.
    /// Provides application services to the commands.
    /// </remarks>
    internal sealed class DiagramToolApplication : IAppServices
    {
        public IHostModelProvider HostModelProvider { get; }
        public IRoslynModelService RoslynModelService { get; }
        public IDiagramService DiagramService { get; }
        public IDiagramWindowService DiagramWindowService { get; }
        public IHostUiService HostUiService { get; }

        public DiagramToolApplication(
            [NotNull] IVisualizationService visualizationService,
            [NotNull] IHostModelProvider hostModelProvider,
            [NotNull] IRoslynModelService roslynModelService,
            [NotNull] IHostUiService hostUiService)
        {
            var diagramId = visualizationService.CreateDiagram();

            DiagramService = visualizationService.GetDiagramService(diagramId);

            DiagramWindowService = (IDiagramWindowService)visualizationService.GetDiagramUiService(diagramId);
            DiagramWindowService.ImageExportDpi = Dpi.Dpi150;
            DiagramWindowService.DiagramNodeInvoked += OnShowSourceRequested;
            DiagramWindowService.ShowModelItemsRequested += OnShowItemsRequested;

            HostModelProvider = hostModelProvider;

            RoslynModelService = roslynModelService;
            RoslynModelService.ExcludeTrivialTypes = AppDefaults.ExcludeTrivialTypes;

            HostUiService = hostUiService;
        }

        private void OnShowSourceRequested(IDiagramShape diagramShape)
        {
            HostUiService.Run(async () => await OnShowSourceRequestAsync(diagramShape));
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
            HostUiService.Run(async () => await OnShowItemsRequestAsync(modelNodes, followWithViewport));
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