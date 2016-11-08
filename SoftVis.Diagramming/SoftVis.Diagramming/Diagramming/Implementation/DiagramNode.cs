using System;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.Diagramming.Implementation
{
    /// <summary>
    /// A diagram node is a vertex in the diagram graph.
    /// It represents a model entity, eg. a box representing a class.
    /// All diagram nodes have a name, a position and a size.
    /// </summary>
    /// <remarks>
    /// Warning: DiagramNode comparison is based on name order, not position or size!
    /// </remarks>
    public class DiagramNode : DiagramShape, IDiagramNode
    {
        private Size2D _size;
        private Point2D _topLeft;

        public event Action<IDiagramNode, Size2D, Size2D> SizeChanged;
        public event Action<IDiagramNode, Point2D, Point2D> TopLeftChanged;

        public DiagramNode(IModelEntity modelEntity)
            : base(modelEntity)
        {
            _size = Size2D.Undefined;
            _topLeft = Point2D.Undefined;
        }

        public IModelEntity ModelEntity => (IModelEntity)ModelItem;
        public string Name => ModelEntity.Name;
        public string FullName => ModelEntity.FullName;
        public override bool IsRectDefined => Size.IsDefined && TopLeft.IsDefined;
        public override Rect2D Rect => new Rect2D(TopLeft, Size);
        public double Width => Size.Width;
        public double Height => Size.Height;

        public virtual Point2D TopLeft
        {
            get { return _topLeft; }
            set
            {
                if (_topLeft != value)
                {
                    var oldTopLeft = _topLeft;
                    _topLeft = value;
                    TopLeftChanged?.Invoke(this, oldTopLeft, value);
                }
            }
        }

        public virtual Size2D Size
        {
            get { return _size; }
            set
            {
                if (_size != value)
                {
                    var oldSize = _size;
                    _size = value;
                    SizeChanged?.Invoke(this, oldSize, value);
                }
            }
        }

        public int Priority => ModelEntity.Priority;

        public Point2D Center
        {
            get { return Rect.Center; }
            set { TopLeft = new Point2D(value.X - Width / 2, value.Y - Height / 2); }
        }

        public int CompareTo(IDiagramNode otherNode)
        {
            return string.Compare(Name, otherNode.Name, StringComparison.InvariantCultureIgnoreCase);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
