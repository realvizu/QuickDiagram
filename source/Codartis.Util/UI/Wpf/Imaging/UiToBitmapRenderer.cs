using System;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Codartis.Util.UI.Wpf.Imaging
{
    /// <summary>
    /// Static helper class that renders WPF UI into bitmap.
    /// </summary>
    public static class UiToBitmapRenderer
    {
        private const double DefaultDpi = 96d;
        private const int RenderingTileSize = 64;

        public static BitmapSource RenderUiElementToBitmap(UIElement uiElement, Rect bounds, double targetDpi,
            CancellationToken cancellationToken = default(CancellationToken),
            IIncrementalProgress progress = null, IProgress<int> maxProgress = null)
        {
            var drawingVisual = new DrawingVisual();
            using (var drawingContext = drawingVisual.RenderOpen())
            {
                DrawContentTiles(uiElement, bounds, drawingContext, RenderingTileSize, RenderingTileSize, cancellationToken, progress, maxProgress);
            }

            var scale = targetDpi / DefaultDpi;
            var imageWidth = bounds.Width * scale;
            var imageHeight = bounds.Height * scale;

            var renderTargetBitmap = new RenderTargetBitmap((int)imageWidth, (int)imageHeight, targetDpi, targetDpi, PixelFormats.Pbgra32);
            renderTargetBitmap.Render(drawingVisual);
            renderTargetBitmap.Freeze();

            return renderTargetBitmap;
        }

        private static void DrawContentTiles(Visual visual, Rect bounds, DrawingContext drawingContext, int tileWidth, int tileHeight,
            CancellationToken cancellationToken, IIncrementalProgress progress, IProgress<int> maxProgress)
        {
            var contentWidth = bounds.Width;
            var contentHeight = bounds.Height;

            var horizontalTileCount = (int)Math.Ceiling(contentWidth / tileWidth);
            var verticalTileCount = (int)Math.Ceiling(contentHeight / tileHeight);
            maxProgress?.Report(horizontalTileCount * verticalTileCount);

            for (var i = 0; i < verticalTileCount; i++)
            {
                for (var j = 0; j < horizontalTileCount; j++)
                {
                    cancellationToken.ThrowIfCancellationRequested();

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

                    progress?.Report(1);
                }
            }
        }
    }
}
