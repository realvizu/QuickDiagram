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
        private readonly object _sizeAndPositionLock = new object();

        private Size2D _size;
        private Point2D _center;
        private string _name;
        private string _fullName;
        private string _description;

        public event Action<IDiagramNode, Size2D, Size2D> SizeChanged;
        public event Action<IDiagramNode, Point2D, Point2D> CenterChanged;
        public event Action<IDiagramNode, string, string, string> Renamed;

        public DiagramNode(IModelEntity modelEntity)
            : base(modelEntity)
        {
            _size = Size2D.Zero;
            _center = Point2D.Undefined;
            _name = modelEntity.Name;
            _fullName = modelEntity.FullName;
            _description = modelEntity.Description;
        }

        public IModelEntity ModelEntity => (IModelEntity)ModelItem;
        public string Name => _name;
        public string FullName => _fullName;
        public string Description => _description;
        public int Priority => ModelEntity.Priority;

        public override bool IsRectDefined => Size.IsDefined && Center.IsDefined;

        public override Rect2D Rect
        {
            get
            {
                lock (_sizeAndPositionLock)
                    return new Rect2D(TopLeft, Size);
            }
        }

        public Point2D Center
        {
            get { return _center; }
            set
            {
                if (_center != value)
                {
                    Point2D oldCenter;

                    lock (_sizeAndPositionLock)
                    {
                        oldCenter = _center;
                        _center = value;
                    }

                    OnCenterChanged(oldCenter, value);
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

                    lock (_sizeAndPositionLock)
                    {
                        oldSize = _size;
                        _size = value;
                    }

                    OnSizeChanged(oldSize, value);
                }
            }
        }

        public double Width => Size.Width;
        public double Height => Size.Height;

        public virtual Point2D TopLeft
        {
            get
            {
                lock (_sizeAndPositionLock)
                    return CenterToTopLeft(_center, _size);
            }
        }

        public void Rename(string name, string fullName, string description)
        {
            _name = name;
            _fullName = fullName;
            _description = description;
            Renamed?.Invoke(this, name, fullName, description);
        }

        public int CompareTo(IDiagramNode otherNode)
        {
            return string.Compare(Name, otherNode.Name, StringComparison.InvariantCultureIgnoreCase);
        }

        public override string ToString()
        {
            return Name;
        }

        private static Point2D CenterToTopLeft(Point2D center, Size2D size)
        {
            return new Point2D(center.X - size.Width / 2, center.Y - size.Height / 2);
        }

        private void OnCenterChanged(Point2D oldCenter, Point2D newCenter)
        {
            CenterChanged?.Invoke(this, oldCenter, newCenter);
        }

        private void OnSizeChanged(Size2D oldSize, Size2D newSize)
        {
            SizeChanged?.Invoke(this, oldSize, newSize);
        }
    }
}
