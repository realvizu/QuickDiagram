namespace Codartis.SoftVis.Diagramming.Events
{
    public abstract class DiagramNodeChangedEventBase : DiagramNodeEventBase
    {
        public IDiagramNode OldNode { get; }

        protected DiagramNodeChangedEventBase(IDiagram newDiagram, IDiagramNode oldNode, IDiagramNode newNode) 
            : base(newDiagram, newNode)
        {
            OldNode = oldNode;
        }
    }
}