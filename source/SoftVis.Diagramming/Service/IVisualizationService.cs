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
        DiagramId CreateDiagram();

        IDiagramUi GetDiagramUi(DiagramId diagramId);

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
