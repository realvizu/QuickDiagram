using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.Diagramming.Shapes
{
    /// <summary>
    /// A diagram node is a vertex in the diagram graph.
    /// It represents a model element which is not a relationship.
    /// Eg. a box representing a class.
    /// All nodes have a name, a position and a size.
    /// </summary>
    public abstract class DiagramNode : DiagramShape
    {
        private readonly string _name;
        private DiagramPoint _position;
        private DiagramSize _size;

        protected DiagramNode(UmlTypeOrPackage typeOrPackage, string name, DiagramPoint position, DiagramSize size)
            :base(typeOrPackage)
        {
            _name = name;
            _position = position;
            _size = size;
        }

        public string Name
        {
            get { return _name; }
        }

        public virtual DiagramPoint Position
        {
            get { return _position; }
            set { _position = value; }
        }

        public virtual DiagramSize Size
        {
            get { return _size; }
            set { _size = value; }
        }

        public UmlTypeOrPackage UmlTypeOrPackage => (UmlTypeOrPackage)ModelElement;

        public override string ToString()
        {
            return Name;
        }
    }
}
