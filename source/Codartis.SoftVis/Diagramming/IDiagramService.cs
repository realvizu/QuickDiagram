using System;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling.Definition;
using JetBrains.Annotations;

namespace Codartis.SoftVis.Diagramming
{
    /// <summary>
    /// Keeps track of the latest diagram instance through mutated instances and publishes change events.
    /// </summary>
    public interface IDiagramService
    {
        [NotNull] IDiagram Diagram { get; }

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

        //IDiagramNode ShowModelNode(IModelNode modelNode);
        //void HideModelNode(ModelNodeId modelNodeId);

        //IReadOnlyList<IDiagramNode> ShowModelNodes(IEnumerable<IModelNode> modelNodes,
        //    CancellationToken cancellationToken = default,
        //    IIncrementalProgress progress = null);

        //void ShowModelRelationship(IModelRelationship modelRelationship);
        //void HideModelRelationship(ModelRelationshipId modelRelationshipId);

        [NotNull]
        ConnectorType GetConnectorType(ModelRelationshipStereotype stereotype);
    }
}