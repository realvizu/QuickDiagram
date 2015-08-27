using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Codartis.SoftVis.Rendering.Wpf.Common
{
    public static class RectExtensions
    {
        public static Point GetCenter(this Rect rect)
        {
            return new Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
        }

        public static Rect Union(this IEnumerable<Rect> rects)
        {
            return rects.Any()
                ? new Rect(
                    new Point(rects.Select(i => i.Left).Min(), rects.Select(i => i.Top).Min()),
                    new Point(rects.Select(i => i.Right).Max(), rects.Select(i => i.Bottom).Max()))
                : Rect.Empty;
        }
    }
}
