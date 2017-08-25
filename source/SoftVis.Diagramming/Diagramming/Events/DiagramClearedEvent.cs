namespace Codartis.SoftVis.Diagramming.Events
{
    public class DiagramClearedEvent : DiagramEventBase
    {
        public DiagramClearedEvent(IDiagram newDiagram) 
            : base(newDiagram)
        {
        }
    }
}