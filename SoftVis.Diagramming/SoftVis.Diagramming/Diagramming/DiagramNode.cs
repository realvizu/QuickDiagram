using System.Diagnostics;
using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.Diagramming
{
    [DebuggerDisplay("{Name}")]
    public abstract class DiagramNode : DiagramShape
    {
        private DiagramRect _rect;

        public string Name { get; private set; }

        protected DiagramNode(UmlModelElement modelElement, string name, DiagramPoint position, DiagramSize size)
            :base(modelElement)
        {
            Name = name;
            _rect = new DiagramRect(position, size);
        }

        public override DiagramRect Rect
        {
            get { return _rect; }
        }

        public void Reposition(DiagramPoint newPosition)
        {
            _rect = new DiagramRect(newPosition, _rect.Size);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
