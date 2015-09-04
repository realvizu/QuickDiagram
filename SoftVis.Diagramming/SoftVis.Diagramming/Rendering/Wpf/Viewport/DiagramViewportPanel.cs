using Codartis.SoftVis.Rendering.Wpf.Common;
using Codartis.SoftVis.Rendering.Wpf.DiagramFixtures;
using Codartis.SoftVis.Rendering.Wpf.DiagramRendering;
using System.Windows;
using System.Windows.Media;

namespace Codartis.SoftVis.Rendering.Wpf.Viewport
{
    /// <summary>
    /// Renders the visible part of a Diagram.
    /// </summary>
    internal partial class DiagramViewportPanel : DiagramPanelBase, IDiagramViewport
    {
        private double _zoom;
        private Size _sizeInScreenSpace;
        private Point _centerInDiagramSpace;

        public Rect ViewportInScreenSpace { get; private set; }
        public Rect ViewportInDiagramSpace { get; private set; }
        public Transform DiagramSpaceToScreenSpace { get; private set; }

        public DiagramViewportPanel()
        {
            _sizeInScreenSpace = Size.Empty;
            _zoom = 1.0;
            _centerInDiagramSpace = new Point(0, 0);

            RecalculateCachedValues();
        }

        public double MinZoom
        {
            get { return (double)GetValue(MinZoomProperty); }
            set { SetValue(MinZoomProperty, value); }
        }

        public double MaxZoom
        {
            get { return (double)GetValue(MaxZoomProperty); }
            set { SetValue(MaxZoomProperty, value); }
        }

        public Rect ContentInDiagramSpace
        {
            get { return DiagramRect; }
        }

        public double Zoom
        {
            get { return _zoom; }
            set
            {
                lock (this)
                {
                    _zoom = value;
                    RecalculateCachedValues();
                }
            }
        }

        public Size SizeInScreenSpace
        {
            get { return _sizeInScreenSpace; }
            set
            {
                lock (this)
                {
                    _sizeInScreenSpace = value;
                    RecalculateCachedValues();
                }
            }
        }

        public Point CenterInDiagramSpace
        {
            get { return _centerInDiagramSpace; }
            set
            {
                lock (this)
                {
                    _centerInDiagramSpace = value;
                    RecalculateCachedValues();
                }
            }
        }

        public void MoveCenterInDiagramSpace(Point newCenterInDiagramSpace)
        {
            CenterInDiagramSpace = newCenterInDiagramSpace;
        }

        public void MoveCenterInScreenSpace(Point newCenterInScreenSpace)
        {
            CenterInDiagramSpace = ProjectPointIntoDiagramSpace(newCenterInScreenSpace);
        }

        public void ResizeInScreenSpace(Size newSizeInScreenSpace)
        {
            SizeInScreenSpace = newSizeInScreenSpace;
        }

        public void ZoomTo(double newZoom)
        {
            Zoom = newZoom;
        }

        public void ZoomWithCenterInScreenSpace(double newZoom, Point zoomCenterInScreenSpace)
        {
            var zoomCenterInDiagramSpace = ProjectPointIntoDiagramSpace(zoomCenterInScreenSpace);
            CenterInDiagramSpace = (CenterInDiagramSpace - zoomCenterInDiagramSpace) * (Zoom / newZoom) + zoomCenterInDiagramSpace;
            Zoom = newZoom;
        }

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            base.ArrangeOverride(arrangeSize);

            foreach (DiagramShapeControlBase child in Children)
                ArrangeChildControl(child);

            return arrangeSize;
        }

        private void ArrangeChildControl(DiagramShapeControlBase child)
        {
            var rect = child.Rect;
            child.Arrange(new Rect(rect.Size));
            child.RenderTransform = CreateTransformForChild(rect.Location);
        }

        private Transform CreateTransformForChild(Point position)
        {
            var transform = new TransformGroup();
            transform.Children.Add(new TranslateTransform(position.X, position.Y));
            transform.Children.Add(DiagramSpaceToScreenSpace);
            return transform;
        }

        private void RecalculateCachedValues()
        {
            ViewportInScreenSpace = new Rect(SizeInScreenSpace);
            ViewportInDiagramSpace = ProjectViewportIntoDiagramSpace();
            DiagramSpaceToScreenSpace = CalculateTransformForDiagramSpaceToScreenSpace(ViewportInDiagramSpace);
        }

        private Point ProjectPointIntoDiagramSpace(Point pointInScreenSpace)
        {
            var screenCenter = new Rect(SizeInScreenSpace).GetCenter();
            var vectorToScreenCenter = screenCenter - pointInScreenSpace;
            var vectorResizedToDiagramSpace = vectorToScreenCenter / Zoom;
            var projectedPoint = CenterInDiagramSpace - vectorResizedToDiagramSpace;
            return projectedPoint;
        }

        private Rect ProjectViewportIntoDiagramSpace()
        {
            if (SizeInScreenSpace.IsEmpty)
                return new Rect(Size.Empty);

            var projectedSize = new Size(SizeInScreenSpace.Width / Zoom, SizeInScreenSpace.Height / Zoom);
            var projectedTopLeft = new Point(CenterInDiagramSpace.X - projectedSize.Width / 2, CenterInDiagramSpace.Y - projectedSize.Height / 2);
            return new Rect(projectedTopLeft, projectedSize);
        }

        private Transform CalculateTransformForDiagramSpaceToScreenSpace(Rect viewportInDiagramSpace)
        {
            var translateVector = (Vector)viewportInDiagramSpace.TopLeft * -1;
            var zoomCenter = new Rect(SizeInScreenSpace).GetCenter();

            var transform = new TransformGroup();
            transform.Children.Add(new TranslateTransform(translateVector.X, translateVector.Y));
            transform.Children.Add(new ScaleTransform(Zoom, Zoom));
            return transform;
        }
    }
}