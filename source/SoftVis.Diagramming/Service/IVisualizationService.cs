using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.UI;
using Codartis.SoftVis.UI.Wpf.View;
using Codartis.SoftVis.Util;

namespace Codartis.SoftVis.Service
{
    /// <summary>
    /// Provides visualization operations and events.
    /// </summary>
    public interface IVisualizationService
    {
        void InitializeUi(IDiagramStlyeProvider diagramStlyeProvider, ResourceDictionary resourceDictionary);

        IModelStore GetModelStore();

        void ClearModel();
        void UpdateModelFromSource();

        DiagramId CreateDiagram(double minZoom, double maxZoom, double initialZoom);

        IDiagramUi GetDiagramUi(DiagramId diagramId);
        void ClearDiagram(DiagramId diagramId);
        void ZoomToContent(DiagramId diagramId);

        void ShowModelNode(DiagramId diagramId, IModelNode modelNode);
        void ShowModelRelationship(DiagramId diagramId, IModelRelationship modelRelationship);

        void HideModelNode(DiagramId diagramId, IModelNode modelNode);
        void HideModelRelationship(DiagramId diagramId, IModelRelationship modelRelationship);

        Task<BitmapSource> CreateDiagramImageAsync(DiagramId diagramId, double dpi, double margin,
            CancellationToken cancellationToken = default(CancellationToken),
            IIncrementalProgress progress = null, IProgress<int> maxProgress = null);

        event Action<IModelNode> ModelNodeInvoked;
    }
}
