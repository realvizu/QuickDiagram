using System.Windows;
using Codartis.SoftVis.Geometry;

namespace Codartis.SoftVis.UI.Wpf
{
    public static class Size2DExtensions
    {
        public static Size ToWpf(this Size2D size2D)
        {
            return new Size(size2D.Width, size2D.Height);
        }
    }
}
