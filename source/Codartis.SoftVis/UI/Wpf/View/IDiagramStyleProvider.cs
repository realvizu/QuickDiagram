using System.Windows;
using System.Windows.Media;

namespace Codartis.SoftVis.UI.Wpf.View
{
    /// <summary>
    /// Provide visual appearance properties of diagrams.
    /// </summary>
    public interface IDiagramStyleProvider
    {
        Brush DiagramFill { get; }
        Brush DiagramStroke { get; }
        Brush Background { get; }
        Brush Foreground { get; }
        FontStyle FontStyle { get; }
        double FontSize { get; }
        FontFamily FontFamily { get; }
        FontStretch FontStretch { get; }
        FontWeight FontWeight { get; }
    }
}
