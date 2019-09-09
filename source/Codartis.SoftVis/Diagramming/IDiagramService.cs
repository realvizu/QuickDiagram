using System;
using System.Collections.Generic;
using System.Threading;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.Util;
using JetBrains.Annotations;

namespace Codartis.SoftVis.Diagramming
{
    /// <summary>
    /// Keeps track of the latest diagram instance through mutated instances and publishes change events.
    /// </summary>
    public interface IDiagramService
    {
        [NotNull] IDiagram LatestDiagram { get; }

        event Action<DiagramEventBase> DiagramChanged;

        void AddNode(ModelNodeId nodeId, ModelNodeId? parentNodeId = null);
        void RemoveNode(ModelNodeId nodeId);
        void AddConnector(ModelRelationshipId relationshipId);
        void RemoveConnector(ModelRelationshipId relationshipId);

        void UpdateModel([NotNull] IModel model);
        void UpdateModelNode([NotNull] IModelNode updatedModelNode);

        void UpdateDiagramNodeSize(ModelNodeId nodeId, Size2D newSize);
        void UpdateDiagramNodeCenter(ModelNodeId nodeId, Point2D newCenter);
        void UpdateDiagramNodeTopLeft(ModelNodeId nodeId, Point2D newTopLeft);
        void UpdateConnectorRoute(ModelRelationshipId relationshipId, Route newRoute);
        void ClearDiagram();

        void AddNodes(
            [NotNull] IEnumerable<ModelNodeId> modelNodeIds,
            CancellationToken cancellationToken = default,
            IIncrementalProgress progress = null);

        ConnectorType GetConnectorType(ModelRelationshipStereotype stereotype);
    }
}