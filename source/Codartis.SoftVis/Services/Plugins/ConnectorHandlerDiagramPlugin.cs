using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.Diagramming.Definition.Events;
using Codartis.SoftVis.Modeling.Definition;
using JetBrains.Annotations;

namespace Codartis.SoftVis.Services.Plugins
{
    /// <summary>
    /// Automatically shows relationships when both ends are visible
    /// and removes redundant connectors from the diagram.
    /// </summary>
    public sealed class ConnectorHandlerDiagramPlugin : ConnectorManipulatorDiagramPluginBase
    {
        public ConnectorHandlerDiagramPlugin(
            [NotNull] IModelService modelService,
            [NotNull] IDiagramService diagramService)
            : base(modelService, diagramService)
        {
            DiagramService.AfterDiagramChanged += OnDiagramChanged;
        }

        public override void Dispose()
        {
            DiagramService.AfterDiagramChanged -= OnDiagramChanged;
        }

        private void OnDiagramChanged(DiagramEvent @event)
        {
            var diagram = @event.NewDiagram;
            var model = diagram.Model;

            foreach (var change in @event.ShapeEvents)
                ProcessDiagramChange(change, model, diagram);
        }

        private void ProcessDiagramChange(DiagramShapeEventBase diagramShapeEvent, IModel model, IDiagram diagram)
        {
            switch (diagramShapeEvent)
            {
                case DiagramNodeAddedEvent diagramNodeAddedEvent:
                    var modelNode = diagramNodeAddedEvent.NewNode.ModelNode;
                    ShowModelRelationshipsIfBothEndsAreVisible(modelNode, model, diagram);
                    break;

                case DiagramNodeRemovedEvent _:
                    // TODO: optimize: should check only surrounding nodes of the deleted one (not all in diagram)
                    foreach (var diagramNode in diagram.Nodes)
                        ShowModelRelationshipsIfBothEndsAreVisible(diagramNode.ModelNode, model, diagram);
                    break;

                case DiagramConnectorAddedEvent _:
                    HideRedundantConnectors(diagram);
                    break;

                // DiagramConnectorRemovedEvent is not handled 
                // because that would put back removed connectors immediately
                // (because nodes are removed after connectors)
            }
        }

        private void HideRedundantConnectors(IDiagram diagram)
        {
            foreach (var connector in diagram.Connectors)
                if (diagram.IsConnectorRedundant(connector.Id))
                    DiagramService.RemoveConnector(connector.Id);
        }
    }
}