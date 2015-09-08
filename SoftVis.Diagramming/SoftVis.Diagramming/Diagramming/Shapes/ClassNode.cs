using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.Diagramming.Shapes
{
    /// <summary>
    /// A node in the diagram graph that represents a class.
    /// </summary>
    public class ClassNode : DiagramNode
    {
        public ClassNode (UmlClass umlClass, string name, DiagramPoint position, DiagramSize size)
            :base(umlClass, name, position, size)
        {
        }

        public UmlClass UmlClass => (UmlClass)ModelElement;
    }
}
