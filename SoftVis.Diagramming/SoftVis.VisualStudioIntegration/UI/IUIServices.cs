using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Codartis.SoftVis.Util.UI.Wpf.Dialogs;
using Codartis.SoftVis.VisualStudioIntegration.ImageExport;

namespace Codartis.SoftVis.VisualStudioIntegration.UI
{
    /// <summary>
    /// Defines the UI operations of the diagram control.
    /// </summary>
    public interface IUiServices
    {
        Dpi ImageExportDpi { get; set; }

        void FitDiagramToView();
        void MessageBox(string message);

        Task<BitmapSource> CreateDiagramImageAsync(ProgressDialog progressDialog = null);
        ProgressDialog ShowProgressDialog(string text);
        string SelectSaveFilename(string title, string filter);
    }
}
