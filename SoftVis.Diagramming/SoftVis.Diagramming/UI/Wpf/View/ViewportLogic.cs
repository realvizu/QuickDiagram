using System.Windows;
using System.Windows.Media;

namespace Codartis.SoftVis.UI.Wpf.View
{
    /// <summary>
    /// Calculates between diagram space and screen space.
    /// </summary>
    internal static class ViewportLogic
    {
        public static Transform CalculateViewportTransform(Size viewportSizeInScreenSpace, double zoom, Point centerInDiagramSpace)
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
    }
}
