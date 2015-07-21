using System.Windows;
using Codartis.SoftVis.Diagramming;

namespace Codartis.SoftVis.Rendering.Wpf.Common
{
    public static class DiagramSizeExtensions
    {
        public static Size ToWpf(this DiagramSize diagramSize)
        {
            return new Size(diagramSize.Width, diagramSize.Height);
        }
    }
}
