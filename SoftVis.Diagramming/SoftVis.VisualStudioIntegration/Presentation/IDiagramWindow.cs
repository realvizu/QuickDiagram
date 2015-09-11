using System.Windows.Media.Imaging;
using Codartis.SoftVis.VisualStudioIntegration.ImageExport;

namespace Codartis.SoftVis.VisualStudioIntegration.Presentation
{
    /// <summary>
    /// Defines the operations of a diagram window.
    /// </summary>
    public interface IDiagramWindow
    {
        void Show();
        void AddCurrentSymbol();
        void Clear();

        int FontSize { get; set; }
        Dpi ImageExportDpi { get; set; }

        BitmapSource GetDiagramAsBitmap();
    }
}
