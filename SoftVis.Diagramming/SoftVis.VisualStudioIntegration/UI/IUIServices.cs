using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Codartis.SoftVis.Util;
using Codartis.SoftVis.Util.UI.Wpf.Dialogs;

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
        ProgressDialog CreateProgressDialog(string text, int maxProgress = 0);
        string SelectSaveFilename(string title, string filter);

        void ShowDiagramWindow();
        void FitDiagramToView();
        void EnsureDiagramContentVisible();
        Task<BitmapSource> CreateDiagramImageAsync(CancellationToken cancellationToken = default(CancellationToken), 
            IIncrementalProgress progress = null, IProgress<int> maxProgress = null);
    }
}
