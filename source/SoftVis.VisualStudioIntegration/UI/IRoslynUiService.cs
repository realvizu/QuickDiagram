using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Codartis.SoftVis.UI.Wpf;
using Codartis.SoftVis.Util;
using Codartis.SoftVis.Util.UI.Wpf.Dialogs;

namespace Codartis.SoftVis.VisualStudioIntegration.UI
{
    /// <summary>
    /// Extends UI operations with roslyn-specific ones.
    /// </summary>
    internal interface IRoslynUiService : IWpfUiService
    {
        Dpi ImageExportDpi { get; set; }

        void ShowDiagramWindow();
        void ShowMessageBox(string message);
        void ShowPopupMessage(string message, TimeSpan hideAfter = default(TimeSpan));
        string SelectSaveFilename(string title, string filter);
        ProgressDialog CreateProgressDialog(string text, int maxProgress = 0);

        void ExpandAllNodes();
        void CollapseAllNodes();

        Task<BitmapSource> CreateDiagramImageAsync(CancellationToken cancellationToken = default(CancellationToken),
            IIncrementalProgress progress = null, IProgress<int> maxProgress = null);
    }
}
