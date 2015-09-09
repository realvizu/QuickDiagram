using System.Windows;
using Codartis.SoftVis.Diagramming;

namespace Codartis.SoftVis.Rendering.Wpf.Common
{
    public static class DiagramRectExtensions
    {
        public static Rect ToWpf(this DiagramRect diagramRect)
        {
            return new Rect(diagramRect.TopLeft.ToWpf(), diagramRect.Size.ToWpf());
        }
    }
}
