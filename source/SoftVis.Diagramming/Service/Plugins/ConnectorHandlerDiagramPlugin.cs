using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Diagramming.Events;
using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.Service.Plugins
{
    /// <summary>
    /// Automatically shows relationships when both ends are visible
    /// and removes redundant connectors from the diagram.
    /// </summary>
    public class ConnectorHandlerDiagramPlugin : ConnectorManipulatorDiagramPluginBase
    {
        public ConnectorHandlerDiagramPlugin(IDiagramShapeFactory diagramShapeFactory)
            : base(diagramShapeFactory)
        {
        }

        public override void Initialize(IReadOnlyModelStore modelStore, IDiagramStore diagramStore)
        {
            base.Initialize(modelStore, diagramStore);

            DiagramStore.DiagramChanged += OnDiagramChanged;
        }

        public override void Dispose()
        {
            DiagramStore.DiagramChanged -= OnDiagramChanged;
        }

        private void OnDiagramChanged(DiagramEventBase diagramEvent)
        {
            var model = ModelStore.CurrentModel;
            var diagram = diagramEvent.NewDiagram;

            switch (diagramEvent)
            {
                case DiagramNodeAddedEvent diagramNodeAddedEvent:
                    var modelNode = diagramNodeAddedEvent.DiagramNode.ModelNode;
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
                if (diagram.IsConnectorRedundantById(connector.Id))
                    DiagramStore.RemoveConnector(connector);
        }
    }
}
