using System.Linq;
using System.Windows;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Util;

namespace Codartis.SoftVis.UI.Wpf
{
    public static class RouteExtensions
    {
        public static Point[] ToWpf(this Route route)
        {
            return route.EmptyIfNull().Select(i => i.ToWpf()).ToArray();
        }
    }
}
