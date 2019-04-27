using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Diagramming.Events;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.Services.Plugins;
using Codartis.SoftVis.VisualStudioIntegration.Modeling;
using Codartis.SoftVis.VisualStudioIntegration.UI;
using Task = System.Threading.Tasks.Task;

namespace Codartis.SoftVis.VisualStudioIntegration.Plugins
{
    /// <summary>
    /// Extends the model with related nodes whenever a diagram node is added.
    /// </summary>
    internal class ModelExtenderDiagramPlugin : DiagramPluginBase
    {
        private readonly IHostUiServices _hostUiServices;

        public ModelExtenderDiagramPlugin(IHostUiServices hostUiServices)
        {
            _hostUiServices = hostUiServices;
        }

        public override void Initialize(IModelService modelService, IDiagramService diagramService)
        {
            base.Initialize(modelService, diagramService);

            DiagramService.DiagramChanged += OnDiagramChanged;
        }

        public override void Dispose()
        {
            DiagramService.DiagramChanged -= OnDiagramChanged;
        }

        private IRoslynModelService RoslynModelService => (IRoslynModelService) ModelService;

        private void OnDiagramChanged(DiagramEventBase diagramEvent)
        {
            _hostUiServices.Run(async () => await OnDiagramChangedAsync(diagramEvent));
        }

        private async Task OnDiagramChangedAsync(DiagramEventBase diagramEvent)
        {
            switch (diagramEvent)
            {
                case DiagramNodeAddedEvent diagramNodeAddedEvent:
                    // It's a fire-and-forget async call, no need to await.
                    // ReSharper disable once UnusedVariable
                    await RoslynModelService.ExtendModelWithRelatedNodesAsync(diagramNodeAddedEvent.DiagramNode.ModelNode, recursive: false);
                    break;
            }
        }
    }
}
