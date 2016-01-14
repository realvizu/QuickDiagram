using System.Linq;
using System.Windows;
using System.Windows.Media;
using Codartis.SoftVis.UI.Common;
using Codartis.SoftVis.UI.Wpf.Common.Geometry;

namespace Codartis.SoftVis.UI.Wpf.View
{
    /// <summary>
    /// Translates between diagram space and screen space.
    /// </summary>
    internal class Viewport
    {
        public Size SizeInScreenSpace { get; private set; }
        public Point CenterInDiagramSpace { get; private set; }
        public double LinearZoom { get; private set; }
        public double MinZoom { get; private set; }
        public double MaxZoom { get; private set; }
        public double DefaultExponentialZoom { get; private set; }

        public double ExponentialZoom { get; private set; }
        public Transform DiagramSpaceToScreenSpace { get; private set; }

        public Viewport(Size sizeInScreenSpace, Point centerInDiagramSpace, double linearZoom, double minZoom, double maxZoom)
        {
            SizeInScreenSpace = sizeInScreenSpace;
            CenterInDiagramSpace = centerInDiagramSpace;
            LinearZoom = linearZoom;
            MinZoom = minZoom;
            MaxZoom = maxZoom;

            DefaultExponentialZoom = ToExponentialZoom(linearZoom);
            UpdateCalculatedProperties();
        }

        public void Resize(Size sizeInScreenSpace)
        {
            SizeInScreenSpace = sizeInScreenSpace;
            UpdateCalculatedProperties();
        }

        public void UpdateDefaultZoom(double defaultLinearZoom)
        {
            DefaultExponentialZoom = ToExponentialZoom(defaultLinearZoom);
            ZoomTo(defaultLinearZoom);
        }

        public void UpdateZoomRange(double minZoom, double maxZoom)
        {
            MinZoom = minZoom;
            MaxZoom = maxZoom;
            UpdateCalculatedProperties();
        }

        public void ZoomTo(double newLinearZoom)
        {
            ZoomWithCenterTo(newLinearZoom, GetScreenCenter(SizeInScreenSpace));
        }

        public void ZoomWithCenterTo(double newLinearZoom, Point zoomCenterInScreenSpace)
        {
            var zoomCenterInDiagramSpace = ProjectToDiagramSpace(zoomCenterInScreenSpace);
            var newExponentialZoom = ToExponentialZoom(newLinearZoom);
            var relativeZoom = ExponentialZoom / newExponentialZoom;

            LinearZoom = newLinearZoom;
            CenterInDiagramSpace = (CenterInDiagramSpace - zoomCenterInDiagramSpace) * relativeZoom + zoomCenterInDiagramSpace;
            UpdateCalculatedProperties();
        }

        public void ZoomToContent(Rect contentRect)
        {
            var exponentialZoom = CalculateZoomForContent(contentRect.Size);
            LinearZoom = ToLinearZoom(exponentialZoom);
            CenterInDiagramSpace = contentRect.GetCenter();
            UpdateCalculatedProperties();
        }

        public void Pan(Vector panVectorInScreenSpace)
        {
            var viewportMoveVectorInScreenSpace = panVectorInScreenSpace * -1;
            var viewportMoveVectorInDiagramSpace = viewportMoveVectorInScreenSpace / ExponentialZoom;

            CenterInDiagramSpace += viewportMoveVectorInDiagramSpace;
            UpdateCalculatedProperties();
        }

        private void UpdateCalculatedProperties()
        {
            ExponentialZoom = ToExponentialZoom(LinearZoom);
            DiagramSpaceToScreenSpace = CreateTransformToScreenSpace(SizeInScreenSpace, ExponentialZoom, CenterInDiagramSpace);
        }

        private double ToExponentialZoom(double linearZoom)
        {
            return ScaleCalculator.LinearToExponential(linearZoom, MinZoom, MaxZoom);
        }

        private double ToLinearZoom(double exponentialZoom)
        {
            return ScaleCalculator.ExponentialToLinear(exponentialZoom, MinZoom, MaxZoom);
        }

        private Point ProjectToDiagramSpace(Point pointInScreenSpace)
        {
            var screenCenter = new Rect(SizeInScreenSpace).GetCenter();
            var vectorToScreenCenter = screenCenter - pointInScreenSpace;
            var vectorResizedToDiagramSpace = vectorToScreenCenter / ExponentialZoom;
            return CenterInDiagramSpace - vectorResizedToDiagramSpace;
        }

        private double CalculateZoomForContent(Size contentSize)
        {
            return new[]
            {
                DefaultExponentialZoom,
                SizeInScreenSpace.Width / contentSize.Width,
                SizeInScreenSpace.Height / contentSize.Height
            }.Min();
        }

        private static Transform CreateTransformToScreenSpace(Size viewportSizeInScreenSpace, double zoom, Point centerInDiagramSpace)
        {
            var viewportInDiagramSpace = ProjectViewportIntoDiagramSpace(viewportSizeInScreenSpace, zoom, centerInDiagramSpace);
            var diagramSpaceToScreenSpace = GetDiagramSpaceToScreenSpaceTransform(viewportInDiagramSpace, zoom);
            return diagramSpaceToScreenSpace;
        }

        private static Rect ProjectViewportIntoDiagramSpace(Size viewportSizeInScreenSpace, double zoom, Point centerInDiagramSpace)
        {
            if (viewportSizeInScreenSpace.IsEmpty)
                return new Rect(Size.Empty);

            var projectedSize = new Size(viewportSizeInScreenSpace.Width / zoom, viewportSizeInScreenSpace.Height / zoom);
            var projectedTopLeft = new Point(centerInDiagramSpace.X - projectedSize.Width / 2, centerInDiagramSpace.Y - projectedSize.Height / 2);
            return new Rect(projectedTopLeft, projectedSize);
        }

        private static Transform GetDiagramSpaceToScreenSpaceTransform(Rect viewportInDiagramSpace, double zoom)
        {
            var translateVector = (Vector)viewportInDiagramSpace.TopLeft * -1;

            var transform = new TransformGroup();
            transform.Children.Add(new TranslateTransform(translateVector.X, translateVector.Y));
            transform.Children.Add(new ScaleTransform(zoom, zoom));
            return transform;
        }

        private static Point GetScreenCenter(Size sizeInScreenSpace)
        {
            return new Rect(sizeInScreenSpace).GetCenter();
        }
    }
}
