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

                case DiagramConnectorRemovedEvent diagramConnectorRemovedEvent:
                    var diagramConnector = diagramConnectorRemovedEvent.DiagramConnector;
                    ShowModelRelationshipsIfBothEndsAreVisible(diagramConnector.Source.ModelNode, model, diagram);
                    ShowModelRelationshipsIfBothEndsAreVisible(diagramConnector.Target.ModelNode, model, diagram);
                    break;
            }
        }
    }
}
