using System.Windows;
using Codartis.SoftVis.Geometry;

namespace Codartis.SoftVis.UI.Wpf.Common.Geometry
{
    public static class Rect2DExtensions
    {
        public static Rect ToWpf(this Rect2D rect2D)
        {
            return new Rect(rect2D.TopLeft.ToWpf(), rect2D.Size.ToWpf());
        }
    }
}
