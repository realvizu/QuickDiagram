using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Codartis.SoftVis.Rendering.Wpf.ImageExport
{
    /// <summary>
    /// Static helper class that renders WPF UI into bitmap.
    /// </summary>
    public static class ImageRenderer
    {
        private const double DefaultDpi = 96d;
        private const int RenderingTileSize = 64;

        public static BitmapSource RenderUIElementToBitmap(UIElement uiElement, double targetDpi)
        {
            Debug.WriteLine("RenderUIElementToBitmap");

            var scale = targetDpi / DefaultDpi;
            var bounds = new Rect(new Point(0, 0), uiElement.DesiredSize);

            var drawingVisual = new DrawingVisual();
            using (var drawingContext = drawingVisual.RenderOpen())
            {
                DrawContentTiles(uiElement, bounds, drawingContext, RenderingTileSize, RenderingTileSize);
            }

            var imageWidth = bounds.Width * scale;
            var imageHeight = bounds.Height * scale;

            var renderTargetBitmap = new RenderTargetBitmap((int)imageWidth, (int)imageHeight, targetDpi, targetDpi, PixelFormats.Pbgra32);
            renderTargetBitmap.Render(drawingVisual);

            return renderTargetBitmap;
        }

        private static void DrawContentTiles(Visual visual, Rect bounds, DrawingContext drawingContext, int tileWidth, int tileHeight)
        {
            var contentWidth = bounds.Width;
            var contentHeight = bounds.Height;

            for (var i = 0; i <= contentHeight / tileHeight; i++)
            {
                for (var j = 0; j <= contentWidth / tileWidth; j++)
                {
                    var targetX = j * tileWidth;
                    var targetY = i * tileHeight;
                    var sourceX = bounds.Left + targetX;
                    var sourceY = bounds.Top + targetY;

                    var width = (j + 1) * tileWidth > contentWidth
                        ? contentWidth - j * tileWidth
                        : tileWidth;
                    var height = (i + 1) * tileHeight > contentHeight
                        ? contentHeight - i * tileHeight
                        : tileHeight;

                    var contentBrush = new VisualBrush(visual)
                    {
                        Stretch = Stretch.None,
                        AlignmentX = AlignmentX.Left,
                        AlignmentY = AlignmentY.Top,
                        Viewbox = new Rect(sourceX, sourceY, width, height),
                        ViewboxUnits = BrushMappingMode.Absolute,
                        Viewport = new Rect(targetX, targetY, width, height),
                        ViewportUnits = BrushMappingMode.Absolute
                    };

                    drawingContext.DrawRectangle(contentBrush, null, new Rect(targetX, targetY, width, height));
                }
            }
        }
    }
}
