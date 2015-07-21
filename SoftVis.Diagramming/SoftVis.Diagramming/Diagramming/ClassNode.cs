using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.Diagramming
{
    public class ClassNode : DiagramNode
    {
        public ClassNode (UmlModelElement modelElement, string name, DiagramPoint position, DiagramSize size)
            :base(modelElement, name, position, size)
        {
        }
    }
}
