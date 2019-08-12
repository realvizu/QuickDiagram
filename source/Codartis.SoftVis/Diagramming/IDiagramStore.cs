using System;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling.Definition;

namespace Codartis.SoftVis.Diagramming
{
    /// <summary>
    /// Keeps track of the latest diagram instance through mutated instances and publishes change events.
    /// </summary>
    public interface IDiagramStore
    {
        IDiagram Diagram { get; }

        event Action<DiagramEventBase> DiagramChanged;

        void AddNode(IDiagramNode node);
        void RemoveNode(ModelNodeId nodeId);
        void UpdateDiagramNodeModelNode(IDiagramNode diagramNode, IModelNode newModelNode);
        void UpdateDiagramNodeSize(IDiagramNode diagramNode, Size2D newSize);
        void UpdateDiagramNodeCenter(IDiagramNode diagramNode, Point2D newCenter);
        void UpdateDiagramNodeTopLeft(IDiagramNode diagramNode, Point2D newTopLeft);
        void AddConnector(IDiagramConnector connector);
        void RemoveConnector(ModelRelationshipId connectorId);
        void UpdateConnectorRoute(ModelRelationshipId connectorId, Route newRoute);
        void ClearDiagram();
    }
}