using System;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling2;

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

        public string DisplayName { get; private set; }
        public string FullName { get; private set; }
        public string Description { get; private set; }
        public string Type { get; }
        public int Priority { get; }

        private Size2D _size;
        private Point2D _center;

        public event Action<IDiagramNode, Size2D, Size2D> SizeChanged;
        public event Action<IDiagramNode, Point2D, Point2D> CenterChanged;
        public event Action<IDiagramNode, string, string, string> Renamed;

        public DiagramNode(IModelNode modelNode)
            : base(modelNode.Id)
        {
            DisplayName = modelNode.DisplayName;
            FullName = modelNode.FullName;
            Description = modelNode.Description;
            Type = modelNode.GetType().Name;
            Priority = modelNode.Priority;

            _size = Size2D.Zero;
            _center = Point2D.Undefined;
        }

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
            DisplayName = name;
            FullName = fullName;
            Description = description;
            Renamed?.Invoke(this, name, fullName, description);
        }

        public int CompareTo(IDiagramNode otherNode) 
            => string.Compare(DisplayName, otherNode.DisplayName, StringComparison.InvariantCultureIgnoreCase);

        public override string ToString() => DisplayName;

        private static Point2D CenterToTopLeft(Point2D center, Size2D size) 
            => new Point2D(center.X - size.Width / 2, center.Y - size.Height / 2);

        private void OnCenterChanged(Point2D oldCenter, Point2D newCenter) => CenterChanged?.Invoke(this, oldCenter, newCenter);
        private void OnSizeChanged(Size2D oldSize, Size2D newSize) => SizeChanged?.Invoke(this, oldSize, newSize);
    }
}
