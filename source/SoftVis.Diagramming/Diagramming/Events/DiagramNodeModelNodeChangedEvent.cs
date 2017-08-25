namespace Codartis.SoftVis.Diagramming.Events
{
    public class DiagramNodeModelNodeChangedEvent : DiagramNodeChangedEventBase
    {
        public DiagramNodeModelNodeChangedEvent(IDiagram newDiagram, IDiagramNode oldNode, IDiagramNode newNode) 
            : base(newDiagram, oldNode, newNode)
        {
        }
    }
}