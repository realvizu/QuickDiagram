using System.Windows;
using Codartis.SoftVis.Geometry;

namespace Codartis.SoftVis.UI.Wpf
{
    public static class Rect2DExtensions
    {
        public static Rect ToWpf(this Rect2D rect2D)
        {
            return new Rect(rect2D.TopLeft.ToWpf(), rect2D.Size.ToWpf());
        }

        public static Rect2D FromWpf(this Rect rect)
        {
            return new Rect2D(rect.Left, rect.Top, rect.Right, rect.Bottom);
        }
    }
}