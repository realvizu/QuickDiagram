using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Diagramming.Events;
using Codartis.SoftVis.Diagramming.Layout;
using Codartis.SoftVis.Diagramming.Layout.Incremental;
using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.Services.Plugins
{
    /// <summary>
    /// A diagram that maintains it own layout.
    /// Responds to shape addition/removal and uses a layout engine that calculates how to arrange nodes and connectors.
    /// </summary>
    public sealed class AutoLayoutDiagramPlugin : DiagramPluginBase
    {
        private readonly ILayoutPriorityProvider _layoutPriorityProvider;
        private IIncrementalLayoutEngine _incrementalLayoutEngine;

        public AutoLayoutDiagramPlugin(ILayoutPriorityProvider layoutPriorityProvider)
        {
            _layoutPriorityProvider = layoutPriorityProvider;
        }

        public override void Initialize(IModelService modelService, IDiagramService diagramService)
        {
            base.Initialize(modelService, diagramService);

            var layoutCalculator = new IncrementalLayoutCalculator(_layoutPriorityProvider);
            _incrementalLayoutEngine = new IncrementalLayoutEngine(layoutCalculator, diagramService);

            DiagramService.DiagramChanged += OnDiagramChanged;
        }

        public override void Dispose()
        {
            DiagramService.DiagramChanged -= OnDiagramChanged;

            _incrementalLayoutEngine?.Dispose();
        }

        private void OnDiagramChanged(DiagramEventBase diagramEvent)
        {
            switch (diagramEvent)
            {
                case DiagramNodeAddedEvent diagramNodeAddedEvent:
                    _incrementalLayoutEngine.EnqueueDiagramAction(new AddDiagramNodeAction(diagramNodeAddedEvent.DiagramNode));
                    break;

                case DiagramConnectorAddedEvent diagramConnectorAddedEvent:
                    _incrementalLayoutEngine.EnqueueDiagramAction(new AddDiagramConnectorAction(diagramConnectorAddedEvent.DiagramConnector));
                    break;

                case DiagramNodeSizeChangedEvent diagramNodeSizeChangedEvent:
                    var diagramNode = diagramNodeSizeChangedEvent.DiagramNode;
                    _incrementalLayoutEngine.EnqueueDiagramAction(new ResizeDiagramNodeAction(diagramNode, diagramNode.Size));
                    break;

                case DiagramNodeRemovedEvent diagramNodeRemovedEvent:
                    _incrementalLayoutEngine.EnqueueDiagramAction(new RemoveDiagramNodeAction(diagramNodeRemovedEvent.DiagramNode));
                    break;

                case DiagramConnectorRemovedEvent diagramConnectorRemovedEvent:
                    _incrementalLayoutEngine.EnqueueDiagramAction(new RemoveDiagramConnectorAction(diagramConnectorRemovedEvent.DiagramConnector));
                    break;

                case DiagramClearedEvent _:
                    _incrementalLayoutEngine.EnqueueDiagramAction(new ClearDiagramAction());
                    break;
            }
        }
    }
}