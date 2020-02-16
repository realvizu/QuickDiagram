using System;
using System.Windows;

namespace Codartis.Util.UI.Wpf
{
    /// <summary>
    /// Defines a position relative to a rectangle.
    /// </summary>
    public struct RectRelativePlacement
    {
        public HorizontalAlignment Horizontal { get; set; }
        public VerticalAlignment Vertical { get; set; }
        public Vector Translate { get; set; }

        public Point GetPositionRelativeTo(Rect rect)
        {
            return GetRelativePoint(rect, Horizontal, Vertical) + Translate;
        }

        private static Point GetRelativePoint(Rect rect, HorizontalAlignment horizontal, VerticalAlignment vertical)
        {
            return new Point(GetRelativeXCoordinate(rect, horizontal), GetRelativeYCoordinate(rect, vertical));
        }

        private static double GetRelativeXCoordinate(Rect rect, HorizontalAlignment horizontalAlignment)
        {
            switch (horizontalAlignment)
            {
                case HorizontalAlignment.Left:
                    return rect.Left;
                case HorizontalAlignment.Center:
                    return rect.Left + rect.Width / 2;
                case HorizontalAlignment.Right:
                    return rect.Left + rect.Width;
                default:
                    throw new ArgumentException($"Unexpected reference point: {horizontalAlignment}");
            }
        }

        private static double GetRelativeYCoordinate(Rect rect, VerticalAlignment verticalAlignment)
        {
            switch (verticalAlignment)
            {
                case VerticalAlignment.Top:
                    return rect.Top;
                case VerticalAlignment.Center:
                    return rect.Top + rect.Height / 2;
                case VerticalAlignment.Bottom:
                    return rect.Top + rect.Height;
                default:
                    throw new ArgumentException($"Unexpected reference point: {verticalAlignment}");
            }
        }
    }
}