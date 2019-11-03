using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Codartis.SoftVis.UI.Wpf;
using Codartis.Util;
using Codartis.Util.UI.Wpf.Dialogs;

namespace Codartis.SoftVis.VisualStudioIntegration.UI
{
    /// <summary>
    /// Extends UI service with application specific operations.
    /// </summary>
    internal interface IApplicationUiService : IWpfUiService
    {
        Dpi ImageExportDpi { get; set; }

        Task ShowDiagramWindowAsync();
        void ShowMessageBox(string message);
        void ShowPopupMessage(string message, TimeSpan hideAfter = default);
        string SelectSaveFilename(string title, string filter);
        Task<ProgressDialog> CreateProgressDialogAsync(string text, int maxProgress = 0);

        void ExpandAllNodes();
        void CollapseAllNodes();

        Task<BitmapSource> CreateDiagramImageAsync(CancellationToken cancellationToken = default,
            IIncrementalProgress progress = null, IProgress<int> maxProgress = null);
    }
}
