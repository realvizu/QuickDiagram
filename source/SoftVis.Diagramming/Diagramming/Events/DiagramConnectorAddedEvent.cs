namespace Codartis.SoftVis.Diagramming.Events
{
    public class DiagramConnectorAddedEvent : DiagramConnectorEventBase
    {
        public DiagramConnectorAddedEvent(IDiagram newDiagram, IDiagramConnector newConnector) 
            : base(newDiagram, newConnector)
        {
        }
    }
}