using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Codartis.SoftVis.Util.UI.Wpf
{
    public static class RectExtensions
    {
        public static Rect Zero = new Rect(0, 0, 0, 0);

        public static bool IsUndefined(this Rect rect)
        {
            return double.IsNaN(rect.Left) || double.IsNaN(rect.Top) || double.IsNaN(rect.Width) || double.IsNaN(rect.Height);
        }

        public static bool IsDefined(this Rect rect) => !rect.IsUndefined();

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

        public static Point GetPerimeterPointTowardPoint(this Rect rect, Point outerPoint)
        {
            var center = rect.GetCenter();
            var sides = new[]
            {
                (rect.Left - outerPoint.X)/(center.X - outerPoint.X),
                (rect.Top - outerPoint.Y)/(center.Y - outerPoint.Y),
                (rect.Right - outerPoint.X)/(center.X - outerPoint.X),
                (rect.Bottom - outerPoint.Y)/(center.Y - outerPoint.Y)
            };

            var fi = Math.Max(0, sides.Where(i => i <= 1).Max());

            return outerPoint + fi * (center - outerPoint);
        }
    }
}
