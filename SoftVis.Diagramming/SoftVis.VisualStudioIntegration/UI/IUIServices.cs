using System;
using System.Windows.Media.Imaging;
using Codartis.SoftVis.VisualStudioIntegration.ImageExport;

namespace Codartis.SoftVis.VisualStudioIntegration.UI
{
    /// <summary>
    /// Defines UI operations for the application commands package.
    /// </summary>
    public interface IUIServices
    {
        int FontSize { get; set; }
        Dpi ImageExportDpi { get; set; }

        void ShowDiagramWindow();
        void FitDiagramToView();
        void GetDiagramImage(Action<BitmapSource> imageCreatedCallback);
    }
}
