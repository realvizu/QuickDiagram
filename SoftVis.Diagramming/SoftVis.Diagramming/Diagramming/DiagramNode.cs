using System;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.Diagramming
{
    /// <summary>
    /// A diagram node is a vertex in the diagram graph.
    /// It represents a model entity, eg. a box representing a class.
    /// All diagram nodes have a name, a position and a size.
    /// </summary>
    /// <remarks>
    /// Warning: DiagramNode comparison is based on name order, not position or size!
    /// </remarks>
    public abstract class DiagramNode : DiagramShape, IComparable<DiagramNode>
    {
        private Point2D _position;
        private Size2D _size;

        protected DiagramNode(IModelEntity modelEntity, Point2D position, Size2D size)
            : base(modelEntity)
        {
            _position = position;
            _size = size;
        }

        public IModelEntity ModelEntity => (IModelEntity)ModelItem;
        public string Name => ModelEntity.Name;
        public Rect2D Rect => new Rect2D(Position, Size);
        public double Width => Size.Width;
        public double Height => Size.Height;

        public virtual Point2D Position
        {
            get { return _position; }
            set { _position = value; }
        }

        public virtual Size2D Size
        {
            get { return _size; }
            set { _size = value; }
        }

        public int Priority => ModelEntity.Priority;

        public Point2D Center
        {
            get { return Rect.Center; }
            set { Position = new Point2D(value.X - Width / 2, value.Y - Height / 2); }
        }

        public int CompareTo(DiagramNode otherNode)
        {
            return string.Compare(Name, otherNode.Name, StringComparison.InvariantCultureIgnoreCase);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
