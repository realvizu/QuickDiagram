using System.Windows;
using System.Windows.Media;

namespace Codartis.SoftVis.UI.Wpf.View
{
    /// <summary>
    /// Stores diagram style properties.
    /// </summary>
    internal class DiagramStyleCache : IDiagramStyleProvider
    {
        public Brush Background { get; }
        public Brush Foreground { get; }
        public Brush DiagramFill { get; }
        public Brush DiagramStroke { get; }
        public FontStyle FontStyle { get; }
        public double FontSize { get; }
        public FontFamily FontFamily { get; }
        public FontStretch FontStretch { get; }
        public FontWeight FontWeight { get; }
        public ResourceDictionary AdditionalResourceDictionary { get; }
        public bool ClipToBounds { get; }
        public bool SnapsToDevicePixels { get; }
        public bool UseLayoutRounding { get; }
        public EdgeMode EdgeMode { get; }
        public ClearTypeHint ClearTypeHint { get; }
        public TextRenderingMode TextRenderingMode { get; }
        public TextHintingMode TextHintingMode { get; }
        public TextFormattingMode TextFormattingMode { get; }

        public DiagramStyleCache(IDiagramStyleProvider diagramStyleProvider)
        {
            Background = diagramStyleProvider.Background;
            Foreground = diagramStyleProvider.Foreground;
            DiagramFill = diagramStyleProvider.DiagramFill;
            DiagramStroke = diagramStyleProvider.DiagramStroke;
            FontStyle = diagramStyleProvider.FontStyle;
            FontSize = diagramStyleProvider.FontSize;
            FontFamily = diagramStyleProvider.FontFamily;
            FontStretch = diagramStyleProvider.FontStretch;
            FontWeight = diagramStyleProvider.FontWeight;
            AdditionalResourceDictionary = diagramStyleProvider.AdditionalResourceDictionary;
            ClipToBounds = diagramStyleProvider.ClipToBounds;
            SnapsToDevicePixels = diagramStyleProvider.SnapsToDevicePixels;
            UseLayoutRounding = diagramStyleProvider.UseLayoutRounding;
            EdgeMode = diagramStyleProvider.EdgeMode;
            ClearTypeHint = diagramStyleProvider.ClearTypeHint;
            TextRenderingMode = diagramStyleProvider.TextRenderingMode;
            TextHintingMode = diagramStyleProvider.TextHintingMode;
            TextFormattingMode = TextFormattingMode;
        }
    }
}