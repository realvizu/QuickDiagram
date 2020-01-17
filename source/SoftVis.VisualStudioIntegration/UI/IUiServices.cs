using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.UI.Wpf.View;
using Codartis.SoftVis.Util;

namespace Codartis.SoftVis.VisualStudioIntegration.UI
{
    /// <summary>
    /// Defines the UI operations of the diagram control.
    /// </summary>
    public interface IUiServices
    {
        DiagramControl DiagramControl { get; }

        Dpi ImageExportDpi { get; set; }

        event Action<IDiagramShape> ShowSourceRequested;
        event Action<IReadOnlyList<IModelEntity>> ShowModelItemsRequested;

        void ShowMessageBox(string message);
        void ShowPopupMessage(string message, TimeSpan hideAfter = default);
        string SelectSaveFilename(string title, string filter);

        void FollowDiagramNode(IDiagramNode diagramNode);
        void FollowDiagramNodes(IReadOnlyList<IDiagramNode> diagramNodes);
        void ZoomToDiagram();
        void KeepDiagramCentered();
        void ExpandAllNodes();
        void CollapseAllNodes();

        Task<BitmapSource> CreateDiagramImageAsync(CancellationToken cancellationToken = default, 
            IIncrementalProgress progress = null, IProgress<int> maxProgress = null);
    }
}
