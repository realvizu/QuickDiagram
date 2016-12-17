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
        private readonly object _lockObject = new object();

        private Size2D _size;
        private Point2D _topLeft;
        private string _name;
        private string _fullName;

        public event Action<IDiagramNode, Size2D, Size2D> SizeChanged;
        public event Action<IDiagramNode, Point2D, Point2D> TopLeftChanged;
        public event Action<IDiagramNode, string, string> Renamed;

        public DiagramNode(IModelEntity modelEntity)
            : base(modelEntity)
        {
            _size = Size2D.Zero;
            _topLeft = Point2D.Undefined;
            _name = modelEntity.Name;
            _fullName = modelEntity.FullName;
        }

        public IModelEntity ModelEntity => (IModelEntity)ModelItem;
        public string Name => _name;
        public string FullName => _fullName;
        public override bool IsRectDefined => Size.IsDefined && TopLeft.IsDefined;
        public override Rect2D Rect => new Rect2D(TopLeft, Size);
        public double Width => Size.Width;
        public double Height => Size.Height;

        public void Rename(string name, string fullName)
        {
            _name = name;
            _fullName = fullName;
            Renamed?.Invoke(this, name, fullName);
        }

        public virtual Point2D TopLeft
        {
            get { return _topLeft; }
            set
            {
                if (_topLeft != value)
                {
                    Point2D oldTopLeft;

                    lock (_lockObject)
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

                    lock (_lockObject)
                    {
                        oldSize = _size;
                        _size = value;
                        TopLeft = AdjustTopLeftForNewSizeWithFixedCenter(oldSize, value);
                    }

                    SizeChanged?.Invoke(this, oldSize, value);
                }
            }
        }

        public int Priority => ModelEntity.Priority;

        public Point2D Center
        {
            get
            {
                lock (_lockObject)
                    return TopLeftToCenter(TopLeft, Size);
            }
            set
            {
                lock (_lockObject)
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
