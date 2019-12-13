using System.Threading.Tasks;
using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.Diagramming.Definition.Events;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.SoftVis.Services.Plugins;
using Codartis.SoftVis.VisualStudioIntegration.Modeling;
using Codartis.SoftVis.VisualStudioIntegration.UI;
using JetBrains.Annotations;

namespace Codartis.SoftVis.VisualStudioIntegration.Plugins
{
    /// <summary>
    /// Extends the model with related nodes whenever a diagram node is added or updated.
    /// </summary>
    internal sealed class ModelExtenderDiagramPlugin : DiagramPluginBase
    {
        [NotNull] private readonly IHostUiService _hostUiServices;
        [NotNull] private readonly IRoslynModelService _roslynModelService;

        public ModelExtenderDiagramPlugin(
            [NotNull] IRoslynModelService roslynModelService,
            [NotNull] IDiagramService diagramService,
            [NotNull] IHostUiService hostUiServices)
            : base(diagramService)
        {
            _hostUiServices = hostUiServices;
            _roslynModelService = roslynModelService;

            DiagramService.DiagramChanged += OnDiagramChanged;
        }

        public override void Dispose()
        {
            DiagramService.DiagramChanged -= OnDiagramChanged;
        }

        private void OnDiagramChanged(DiagramEvent diagramEvent)
        {
            _hostUiServices.Run(async () => await OnDiagramChangedAsync(diagramEvent));
        }

        [NotNull]
        private async Task OnDiagramChangedAsync(DiagramEvent diagramEvent)
        {
            foreach (var shapeEvent in diagramEvent.ShapeEvents)
            {
                switch (shapeEvent)
                {
                    case DiagramNodeAddedEvent diagramNodeAddedEvent:
                        await ExtendModelNodeAsync(diagramNodeAddedEvent.NewNode.ModelNode);
                        break;

                    case DiagramNodeChangedEvent diagramNodeChangedEvent when diagramNodeChangedEvent.ChangedMember == DiagramNodeMember.ModelNode:
                        await ExtendModelNodeAsync(diagramNodeChangedEvent.NewNode.ModelNode);
                        break;
                }
            }
        }

        [NotNull]
        private async Task ExtendModelNodeAsync([NotNull] IModelNode modelNode)
        {
            await _roslynModelService.ExtendModelWithRelatedNodesAsync(modelNode, recursive: false);
        }
    }
}