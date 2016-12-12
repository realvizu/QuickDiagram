using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Modeling;
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

        event Action<IDiagramShape> ShowSourceRequested;
        event Action<List<IModelEntity>> ShowModelItemsRequested;

        void ShowDiagramWindow();
        void ShowMessageBox(string message);
        void ShowPopupMessage(string message, TimeSpan hideAfter = default(TimeSpan));
        string SelectSaveFilename(string title, string filter);

        void ExecuteWhenUiIsIdle(Action action);
        void FollowDiagramNodes(IEnumerable<IDiagramNode> diagramNodes);
        void StopFollowingDiagramNodes();
        void ZoomToDiagram();
        void ZoomToDiagramNode(IDiagramNode diagramNode);
        void ZoomToDiagramNodes(IEnumerable<IDiagramNode> diagramNodes);
        void EnsureDiagramVisible();

        ProgressDialog CreateProgressDialog(string text, int maxProgress = 0);
        Task<BitmapSource> CreateDiagramImageAsync(CancellationToken cancellationToken = default(CancellationToken), 
            IIncrementalProgress progress = null, IProgress<int> maxProgress = null);
    }
}
