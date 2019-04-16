namespace Codartis.SoftVis.Diagramming.Events
{
    public class DiagramConnectorRemovedEvent : DiagramConnectorEventBase
    {
        public DiagramConnectorRemovedEvent(IDiagram newDiagram, IDiagramConnector removedConnector) 
            : base(newDiagram, removedConnector)
        {
        }
    }
}