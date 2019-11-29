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
        ResourceDictionary AdditionalResourceDictionary { get; }
        bool ClipToBounds { get; }
        bool SnapsToDevicePixels { get; }
        bool UseLayoutRounding { get; }
        EdgeMode EdgeMode { get; }
        ClearTypeHint ClearTypeHint { get; }
        TextRenderingMode TextRenderingMode { get; }
        TextHintingMode TextHintingMode { get; }
        TextFormattingMode TextFormattingMode { get; }
    }
}