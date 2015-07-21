using System.Windows;
using Codartis.SoftVis.Diagramming;

namespace Codartis.SoftVis.Rendering.Wpf.Common
{
    public static class DiagramPointExtensions
    {
        public static Point ToWpf(this DiagramPoint diagramPoint)
        {
            return new Point(diagramPoint.X, diagramPoint.Y);
        }
    }
}
