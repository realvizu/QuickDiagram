using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Codartis.SoftVis.Util.UI.Wpf.Dialogs;
using Codartis.SoftVis.Util.UI.Wpf.ViewModels;

namespace Codartis.SoftVis.VisualStudioIntegration.UI
{
    /// <summary>
    /// Defines the UI operations of the diagram control.
    /// </summary>
    public interface IUiServices
    {
        Dpi ImageExportDpi { get; set; }

        void ShowMessageBox(string message);
        void ShowPopupMessage(string message, TimeSpan hideAfter = default(TimeSpan));
        ProgressDialog ShowProgressDialog(string text, ProgressMode progressMode = ProgressMode.Percentage);
        string SelectSaveFilename(string title, string filter);

        void ShowDiagramWindow();
        void FitDiagramToView();
        Task<BitmapSource> CreateDiagramImageAsync(CancellationToken cancellationToken = default(CancellationToken), IProgress<double> progress = null);
    }
}
