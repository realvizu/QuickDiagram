using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.Diagramming
{
    public class InterfaceNode : DiagramNode
    {
        public InterfaceNode(UmlModelElement modelElement, string name, DiagramPoint position, DiagramSize size)
            :base(modelElement, name, position, size)
        {
        }
    }
}
