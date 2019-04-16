namespace Codartis.SoftVis.Diagramming.Events
{
    public class DiagramConnectorRouteChangedEvent : DiagramConnectorEventBase
    {
        public IDiagramConnector NewConnector { get; }

        public DiagramConnectorRouteChangedEvent(IDiagram newDiagram, IDiagramConnector oldConnector, IDiagramConnector newConnector) 
            : base(newDiagram, oldConnector)
        {
            NewConnector = newConnector;
        }
    }
}