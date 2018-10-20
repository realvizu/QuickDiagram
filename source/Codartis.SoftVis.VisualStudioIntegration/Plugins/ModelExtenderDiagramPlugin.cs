using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Diagramming.Events;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.Service.Plugins;
using Codartis.SoftVis.VisualStudioIntegration.Modeling;

namespace Codartis.SoftVis.VisualStudioIntegration.Plugins
{
    /// <summary>
    /// Extends the model with related nodes whenever a diagram node is added.
    /// </summary>
    internal class ModelExtenderDiagramPlugin : DiagramPluginBase
    {
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
            switch (diagramEvent)
            {
                case DiagramNodeAddedEvent diagramNodeAddedEvent:
                    // It's a fire-and-forget async call, no need to await.
                    var task = RoslynModelService.ExtendModelWithRelatedNodesAsync(diagramNodeAddedEvent.DiagramNode.ModelNode, recursive: false);
                    break;
            }
        }
    }
}
