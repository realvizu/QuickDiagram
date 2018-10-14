namespace Codartis.SoftVis.Diagramming.Events
{
    public abstract class DiagramNodeEventBase : DiagramEventBase
    {
        public IDiagramNode DiagramNode { get; }

        protected DiagramNodeEventBase(IDiagram newDiagram, IDiagramNode diagramNode) 
            : base(newDiagram)
        {
            DiagramNode = diagramNode;
        }
    }
}