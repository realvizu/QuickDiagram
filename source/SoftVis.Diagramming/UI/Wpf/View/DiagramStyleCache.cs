using System.Windows;
using System.Windows.Media;

namespace Codartis.SoftVis.UI.Wpf.View
{
    /// <summary>
    /// Stores diagram style properties.
    /// </summary>
    internal class DiagramStyleCache : IDiagramStlyeProvider
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

        public DiagramStyleCache(IDiagramStlyeProvider diagramStlyeProvider)
        {
            Background = diagramStlyeProvider.Background;
            Foreground = diagramStlyeProvider.Foreground;
            DiagramFill = diagramStlyeProvider.DiagramFill;
            DiagramStroke = diagramStlyeProvider.DiagramStroke;
            FontStyle = diagramStlyeProvider.FontStyle;
            FontSize = diagramStlyeProvider.FontSize;
            FontFamily = diagramStlyeProvider.FontFamily;
            FontStretch = diagramStlyeProvider.FontStretch;
            FontWeight = diagramStlyeProvider.FontWeight;
        }
    }
}
