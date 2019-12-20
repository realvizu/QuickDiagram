using System.Collections.Generic;
using System.Linq;
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
        public IRoslynWorkspaceProvider RoslynWorkspaceProvider { get; }
        public IRoslynBasedModelService RoslynBasedModelService { get; }
        public IDiagramService DiagramService { get; }
        public IDiagramWindowService DiagramWindowService { get; }
        public IHostUiService HostUiService { get; }

        public DiagramToolApplication(
            [NotNull] IVisualizationService visualizationService,
            [NotNull] IRoslynWorkspaceProvider roslynWorkspaceProvider,
            [NotNull] IRoslynBasedModelService roslynBasedModelService,
            [NotNull] IHostUiService hostUiService)
        {
            var diagramId = visualizationService.CreateDiagram();

            DiagramService = visualizationService.GetDiagramService(diagramId);

            DiagramWindowService = (IDiagramWindowService)visualizationService.GetDiagramUiService(diagramId);
            DiagramWindowService.ImageExportDpi = Dpi.Dpi150;
            DiagramWindowService.DiagramNodeInvoked += OnShowSourceRequested;
            DiagramWindowService.ShowModelItemsRequested += OnShowItemsRequested;

            RoslynWorkspaceProvider = roslynWorkspaceProvider;

            RoslynBasedModelService = roslynBasedModelService;
            RoslynBasedModelService.ExcludeTrivialTypes = AppDefaults.ExcludeTrivialTypes;

            HostUiService = hostUiService;
        }

        private void OnShowSourceRequested([NotNull] IDiagramShape diagramShape)
        {
            HostUiService.Run(async () => await OnShowSourceRequestAsync(diagramShape));
        }

        [NotNull]
        private async Task OnShowSourceRequestAsync([NotNull] IDiagramShape diagramShape)
        {
            if (diagramShape is IDiagramNode diagramNode)
                await new ShowSourceFileCommand(this, diagramNode).ExecuteAsync();
        }

        private void OnShowItemsRequested([NotNull] IReadOnlyList<IModelNode> modelNodes, bool followWithViewport)
        {
            HostUiService.Run(async () => await OnShowItemsRequestAsync(modelNodes, followWithViewport));
        }

        [NotNull]
        private async Task OnShowItemsRequestAsync([NotNull] IReadOnlyList<IModelNode> modelNodes, bool followWithViewport)
        {
            var modelNodeIds = modelNodes.Select(i => i.Id).ToArray();
            if (modelNodeIds.Any())
                await new AddItemsToDiagramCommand(this, modelNodeIds, followWithViewport).ExecuteAsync();
        }
    }
}