using System;
using System.Collections.Generic;
using System.Threading;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.Util;
using JetBrains.Annotations;

namespace Codartis.SoftVis.Diagramming.Definition
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

        void UpdatePayloadAreaSize(ModelNodeId nodeId, Size2D newSize);
        void UpdateChildrenAreaSize(ModelNodeId nodeId, Size2D newSize);
        void UpdateCenter(ModelNodeId nodeId, Point2D newCenter);
        void UpdateTopLeft(ModelNodeId nodeId, Point2D newTopLeft);
        void UpdateRoute(ModelRelationshipId relationshipId, Route newRoute);
        void ClearDiagram();

        void ApplyLayout(DiagramLayoutInfo diagramLayout);

        /// <summary>
        /// Adds multiple nodes to the diagram.
        /// Those nodes whose parent are already on the diagram are added to their parents.
        /// </summary>
        /// <remarks>
        /// WARNING: if the child is added before the parent then the parent won't contain the child.
        /// </remarks>
        void AddNodes(
            [NotNull] IEnumerable<ModelNodeId> modelNodeIds,
            CancellationToken cancellationToken = default,
            IIncrementalProgress progress = null);

        void AddConnectors(
            [NotNull] IEnumerable<ModelRelationshipId> modelRelationshipIds,
            CancellationToken cancellationToken = default,
            IIncrementalProgress progress = null);

        ConnectorType GetConnectorType(ModelRelationshipStereotype stereotype);
    }
}