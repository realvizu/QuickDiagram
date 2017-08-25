namespace Codartis.SoftVis.Diagramming.Events
{
    public class DiagramNodeAddedEvent: DiagramNodeEventBase
    {
        public DiagramNodeAddedEvent(IDiagram newDiagram, IDiagramNode newNode) 
            : base(newDiagram, newNode)
        {
        }
    }
}