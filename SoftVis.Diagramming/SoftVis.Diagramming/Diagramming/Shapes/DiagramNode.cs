using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.Diagramming.Shapes
{
    /// <summary>
    /// A diagram node is a vertex in the diagram graph.
    /// It represents a model entity, eg. a box representing a class.
    /// All diagram nodes have a name, a position and a size.
    /// </summary>
    public abstract class DiagramNode : DiagramShape
    {
        private DiagramPoint _position;
        private DiagramSize _size;

        protected DiagramNode(IModelEntity modelEntity, DiagramPoint position, DiagramSize size)
            :base(modelEntity)
        {
            _position = position;
            _size = size;
        }

        public IModelEntity ModelEntity => (IModelEntity)ModelItem;
        public string Name => ModelEntity.Name;
        public DiagramRect Rect => new DiagramRect(Position, Size);

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
