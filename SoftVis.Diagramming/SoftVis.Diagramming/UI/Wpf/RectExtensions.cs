using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Codartis.SoftVis.UI.Geometry;

namespace Codartis.SoftVis.UI.Wpf
{
    public static class RectExtensions
    {
        public static Point GetCenter(this Rect rect)
        {
            return new Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
        }

        public static Rect Add(this Rect rect, Vector vector)
        {
            return new Rect(rect.TopLeft + vector, rect.Size);
        }

        public static Rect Union(this IEnumerable<Rect> rectCollection)
        {
            var rectList = rectCollection as IList<Rect> ?? rectCollection.ToList();
            return rectList.Any()
                ? new Rect(
                    new Point(rectList.Select(i => i.Left).Min(), rectList.Select(i => i.Top).Min()),
                    new Point(rectList.Select(i => i.Right).Max(), rectList.Select(i => i.Bottom).Max()))
                : Rect.Empty;
        }

        public static Point GetRelativePoint(this Rect rect, RectRelativeLocation rectRelativeLocation)
        {
            return rect.GetRelativePoint(rectRelativeLocation.Alignment) + rectRelativeLocation.Translate.ToVector();
        }

        public static Point GetRelativePoint(this Rect rect, RectAlignment rectAlignment)
        {
            return new Point(rect.GetRelativeXCoordinate(rectAlignment), rect.GetRelativeYCoordinate(rectAlignment));
        }

        private static double GetRelativeXCoordinate(this Rect rect, RectAlignment rectAlignment)
        {
            switch (rectAlignment.HorizontalAlignment)
            {
                case HorizontalAlignmentType.Left: return rect.X;
                case HorizontalAlignmentType.Middle: return rect.X + rect.Width / 2;
                case HorizontalAlignmentType.Right: return rect.X + rect.Width;
                default: throw new ArgumentException($"Unexpected reference point: {rectAlignment}");
            }
        }

        private static double GetRelativeYCoordinate(this Rect rect, RectAlignment rectAlignment)
        {
            switch (rectAlignment.VerticalAlignment)
            {
                case VerticalAlignmentType.Top: return rect.Y;
                case VerticalAlignmentType.Middle: return rect.Y + rect.Height / 2;
                case VerticalAlignmentType.Bottom: return rect.Y + rect.Height;
                default: throw new ArgumentException($"Unexpected reference point: {rectAlignment}");
            }
        }
    }
}
