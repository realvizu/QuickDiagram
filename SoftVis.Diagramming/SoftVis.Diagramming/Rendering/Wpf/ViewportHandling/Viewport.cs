using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using Codartis.SoftVis.Rendering.Wpf.Common;

namespace Codartis.SoftVis.Rendering.Wpf.ViewportHandling
{
    public class Viewport
    {
        private Size _sizeInScreenSpace;
        private Point _centerInDiagramSpace;
        private double _zoom;

        public Rect ViewportInScreenSpace { get; private set; }
        public Rect ViewportInDiagramSpace { get; private set; }
        public Transform DiagramSpaceToScreenSpace { get; private set; }

        public Viewport()
        {
            _sizeInScreenSpace = Size.Empty;
            _zoom = 1.0;
            _centerInDiagramSpace = new Point(0, 0);
            RecalculateCachedValues();
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
            CenterInDiagramSpace = (CenterInDiagramSpace - zoomCenterInDiagramSpace) * (Zoom/newZoom) + zoomCenterInDiagramSpace;
            Zoom = newZoom;
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
