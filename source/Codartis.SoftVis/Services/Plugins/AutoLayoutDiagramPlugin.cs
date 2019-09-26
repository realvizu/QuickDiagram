using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.Diagramming.Definition.Events;
using Codartis.SoftVis.Diagramming.Definition.Layout;
using Codartis.SoftVis.Modeling.Definition;
using JetBrains.Annotations;

namespace Codartis.SoftVis.Services.Plugins
{
    /// <summary>
    /// Listens to diagram modification events and performs layout.
    /// </summary>
    public sealed class AutoLayoutDiagramPlugin : DiagramPluginBase
    {
        [NotNull] private readonly IDiagramLayoutAlgorithm _layoutAlgorithm;

        public AutoLayoutDiagramPlugin([NotNull] IDiagramLayoutAlgorithm layoutAlgorithm)
        {
            _layoutAlgorithm = layoutAlgorithm;
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

        private void OnDiagramChanged([NotNull] DiagramEventBase diagramEvent)
        {
            switch (diagramEvent)
            {
                case DiagramNodeRectChangedEvent _:
                case DiagramConnectorRouteChangedEvent _:
                    return;
            }

            var diagram = diagramEvent.NewDiagram;
            var diagramLayoutInfo = _layoutAlgorithm.Calculate(diagram);
            DiagramService.ApplyLayout(diagramLayoutInfo);
        }
    }
}