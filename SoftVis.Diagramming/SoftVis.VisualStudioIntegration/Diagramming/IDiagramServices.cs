using System.Windows.Media.Imaging;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.VisualStudioIntegration.ImageExport;

namespace Codartis.SoftVis.VisualStudioIntegration.Diagramming
{
    /// <summary>
    /// Defines diagram operations for the application commands package.
    /// </summary>
    public interface IDiagramServices
    {
        int FontSize { get; set; }
        Dpi ImageExportDpi { get; set; }

        void ShowDiagram();
        void ShowModelEntity(IModelEntity modelEntity);
        void FitDiagramToView();
        void ClearDiagram();
        BitmapSource GetDiagramAsBitmap();

    }
}
