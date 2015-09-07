using System.Diagnostics;
using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.Diagramming
{
    [DebuggerDisplay("{Name}")]
    public abstract class DiagramNode : DiagramShape
    {
        private readonly string _name;
        private DiagramPoint _position;
        private DiagramSize _size;

        protected DiagramNode(UmlModelElement modelElement, string name, DiagramPoint position, DiagramSize size)
            :base(modelElement)
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

        public override string ToString()
        {
            return Name;
        }
    }
}
