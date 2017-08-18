using System;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.Diagramming.Implementation
{
    /// <summary>
    /// A mutable implementation of the IDiagramNode interface.
    /// </summary>
    public class DiagramNode : DiagramShape, IDiagramNode
    {
        private Size2D _size;
        private Point2D _center;
        private readonly object _sizeAndPositionLock = new object();

        public event Action<IDiagramNode, Size2D, Size2D> SizeChanged;
        public event Action<IDiagramNode, Point2D, Point2D> CenterChanged;
        public event Action<IDiagramNode, IModelNode> ModelNodeUpdated;

        public DiagramNode(IModelNode modelNode)
            : base(modelNode)
        {
            _size = Size2D.Zero;
            _center = Point2D.Undefined;
        }

        public IModelNode ModelNode => (IModelNode)ModelItem;
        public string Name => ModelNode.Name;
        public int LayoutPriority => ModelNode.LayoutPriority;

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
                    return Rect2D.CreateFromCenterAndSize(_center, _size).TopLeft;
            }
        }

        public override void Update(IModelItem modelItem)
        {
            base.Update(modelItem);
            ModelNodeUpdated?.Invoke(this, (IModelNode)modelItem);
        }

        public int CompareTo(IDiagramNode otherNode) =>
            string.Compare(Name, otherNode.Name, StringComparison.InvariantCultureIgnoreCase);

        public override string ToString() => Name;

        private void OnCenterChanged(Point2D oldCenter, Point2D newCenter) =>
            CenterChanged?.Invoke(this, oldCenter, newCenter);

        private void OnSizeChanged(Size2D oldSize, Size2D newSize) =>
            SizeChanged?.Invoke(this, oldSize, newSize);
    }
}
