namespace Codartis.SoftVis.Diagramming.Events
{
    public class DiagramNodePositionChangedEvent : DiagramNodeChangedEventBase
    {
        public DiagramNodePositionChangedEvent(IDiagram newDiagram, IDiagramNode oldNode, IDiagramNode newNode) 
            : base(newDiagram, oldNode, newNode)
        {
        }
    }
}