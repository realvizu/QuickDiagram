using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.Diagramming.Shapes
{
    public class InterfaceNode : DiagramNode
    {
        /// <summary>
        /// A node in the diagram graph that represents an interface.
        /// </summary>
        public InterfaceNode(UmlInterface umlInterface, string name, DiagramPoint position, DiagramSize size)
            :base(umlInterface, name, position, size)
        {
        }

        public UmlInterface UmlInterface => (UmlInterface)ModelElement;
    }
}
