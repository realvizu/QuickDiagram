using System;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.UI;

namespace Codartis.SoftVis.Service
{
    /// <summary>
    /// Provides visualization operations and events.
    /// </summary>
    public interface IVisualizationService
    {
        IModelStore GetModelStore();

        DiagramId CreateDiagram();
        IDiagramUi CreateDiagramUi(DiagramId diagramId, double minZoom, double maxZoom, double initialZoom);

        void ShowModelNode(DiagramId diagramId, IModelNode modelNode);
        void ShowModelRelationship(DiagramId diagramId, IModelRelationship modelRelationship);

        void HideModelNode(DiagramId diagramId, IModelNode modelNode);
        void HideModelRelationship(DiagramId diagramId, IModelRelationship modelRelationship);

        void ClearDiagram(DiagramId diagramId);

        void ClearModel();
        void UpdateModelFromSource();

        event Action<IModelNode> ModelNodeInvoked;
    }
}
