using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Codartis.SoftVis.UI.Wpf;
using Codartis.SoftVis.UI.Wpf.View;
using Codartis.SoftVis.Util;
using Codartis.SoftVis.Util.UI.Wpf.Dialogs;

namespace Codartis.SoftVis.VisualStudioIntegration.UI
{
    /// <summary>
    /// Extends UI service with application specific operations.
    /// </summary>
    internal interface IApplicationUiService : IWpfUiService
    {
        DiagramControl DiagramControl { get; }

        Dpi ImageExportDpi { get; set; }

        Task ShowDiagramWindowAsync();
        void ShowMessageBox(string message);
        void ShowPopupMessage(string message, TimeSpan hideAfter = default(TimeSpan));
        string SelectSaveFilename(string title, string filter);
        Task<ProgressDialog> CreateProgressDialogAsync(string text, int maxProgress = 0);

        void ExpandAllNodes();
        void CollapseAllNodes();

        Task<BitmapSource> CreateDiagramImageAsync(CancellationToken cancellationToken = default(CancellationToken),
            IIncrementalProgress progress = null, IProgress<int> maxProgress = null);
    }
}
