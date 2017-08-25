namespace Codartis.SoftVis.Diagramming.Events
{
    public class DiagramNodeSizeChangedEvent : DiagramNodeChangedEventBase
    {
        public DiagramNodeSizeChangedEvent(IDiagram newDiagram, IDiagramNode oldNode, IDiagramNode newNode) 
            : base(newDiagram, oldNode, newNode)
        {
        }
    }
}