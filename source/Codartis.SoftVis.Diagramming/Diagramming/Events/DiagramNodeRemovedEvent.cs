namespace Codartis.SoftVis.Diagramming.Events
{
    public class DiagramNodeRemovedEvent : DiagramNodeEventBase
    {
        public DiagramNodeRemovedEvent(IDiagram newDiagram, IDiagramNode removedNode) 
            : base(newDiagram, removedNode)
        {
        }
    }
}