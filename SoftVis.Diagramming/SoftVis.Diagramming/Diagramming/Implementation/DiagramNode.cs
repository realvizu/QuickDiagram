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
            _size = Size2D.Zero;
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
                    Point2D oldTopLeft;

                    lock (this)
                    {
                         oldTopLeft = _topLeft;
                        _topLeft = value;
                    }

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
                    Size2D oldSize;
                    Point2D newTopLeft;

                    lock (this)
                    {
                        oldSize = _size;
                        _size = value;
                        newTopLeft = AdjustTopLeftForNewSizeWithFixedCenter(oldSize, value);
                    }

                    SizeChanged?.Invoke(this, oldSize, value);
                    TopLeft = newTopLeft;
                }
            }
        }

        public int Priority => ModelEntity.Priority;

        public Point2D Center
        {
            get
            {
                lock (this)
                    return TopLeftToCenter(TopLeft, Size);
            }
            set
            {
                lock (this)
                    TopLeft = CenterToTopLeft(value, Size);
            }
        }

        public int CompareTo(IDiagramNode otherNode)
        {
            return string.Compare(Name, otherNode.Name, StringComparison.InvariantCultureIgnoreCase);
        }

        public override string ToString()
        {
            return Name;
        }

        private Point2D AdjustTopLeftForNewSizeWithFixedCenter(Size2D oldSize, Size2D newSize)
        {
            var center = TopLeftToCenter(TopLeft, oldSize);
            return CenterToTopLeft(center, newSize);
        }

        private static Point2D CenterToTopLeft(Point2D center, Size2D size)
        {
            return new Point2D(center.X - size.Width / 2, center.Y - size.Height / 2);
        }

        private static Point2D TopLeftToCenter(Point2D topLeft, Size2D size)
        {
            return new Point2D(topLeft.X + size.Width / 2, topLeft.Y + size.Height / 2);
        }
    }
}
