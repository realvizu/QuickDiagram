namespace Codartis.SoftVis.Diagramming
{
    public abstract class DiagramEventBase
    {
        public IDiagram NewDiagram { get; }

        protected DiagramEventBase(IDiagram newDiagram)
        {
            NewDiagram = newDiagram;
        }
    }
}