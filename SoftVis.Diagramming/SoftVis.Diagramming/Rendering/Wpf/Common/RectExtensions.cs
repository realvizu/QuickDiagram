using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Codartis.SoftVis.Geometry;

namespace Codartis.SoftVis.Rendering.Wpf.Common
{
    public static class RectExtensions
    {
        public static Point GetCenter(this Rect rect)
        {
            return new Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
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
            return rect.GetRelativePoint(rectRelativeLocation.ReferencePoint) + rectRelativeLocation.Translate.ToVector();
        }

        public static Point GetRelativePoint(this Rect rect, RectReferencePoint rectReferencePoint)
        {
            return new Point(rect.GetRelativeXCoordinate(rectReferencePoint), rect.GetRelativeYCoordinate(rectReferencePoint));
        }

        private static double GetRelativeXCoordinate(this Rect rect, RectReferencePoint rectReferencePoint)
        {
            switch (rectReferencePoint)
            {
                case RectReferencePoint.TopLeft:
                case RectReferencePoint.CenterLeft:
                case RectReferencePoint.BottomLeft:
                    return rect.X;

                case RectReferencePoint.TopCenter:
                case RectReferencePoint.Center:
                case RectReferencePoint.BottomCenter:
                    return rect.X + rect.Width / 2;

                case RectReferencePoint.TopRight:
                case RectReferencePoint.CenterRight:
                case RectReferencePoint.BottomRight:
                    return rect.X + rect.Width;

                default:
                    throw new ArgumentException($"Unexpected reference point: {rectReferencePoint}");
            }
        }

        private static double GetRelativeYCoordinate(this Rect rect, RectReferencePoint rectReferencePoint)
        {
            switch (rectReferencePoint)
            {
                case RectReferencePoint.TopLeft:
                case RectReferencePoint.TopCenter:
                case RectReferencePoint.TopRight:
                    return rect.Y;

                case RectReferencePoint.CenterLeft:
                case RectReferencePoint.Center:
                case RectReferencePoint.CenterRight:
                    return rect.Y + rect.Height / 2;

                case RectReferencePoint.BottomLeft:
                case RectReferencePoint.BottomCenter:
                case RectReferencePoint.BottomRight:
                    return rect.Y + rect.Height;

                default:
                    throw new ArgumentException($"Unexpected reference point: {rectReferencePoint}");
            }
        }
    }
}
