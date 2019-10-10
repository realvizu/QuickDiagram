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
    public interface IDiagramService : IDiagramEventSource
    {
        [NotNull] IDiagram LatestDiagram { get; }

        void AddNode(ModelNodeId nodeId, ModelNodeId? parentNodeId = null);
        void UpdateNodePayloadAreaSize(ModelNodeId nodeId, Size2D newSize);
        void UpdateNodeChildrenAreaSize(ModelNodeId nodeId, Size2D newSize);
        void UpdateNodeCenter(ModelNodeId nodeId, Point2D newCenter);
        void UpdateNodeTopLeft(ModelNodeId nodeId, Point2D newTopLeft);
        void RemoveNode(ModelNodeId nodeId);

        void AddConnector(ModelRelationshipId relationshipId);
        void UpdateConnectorRoute(ModelRelationshipId relationshipId, Route newRoute);
        void RemoveConnector(ModelRelationshipId relationshipId);

        void UpdateModel([NotNull] IModel model);
        void UpdateModelNode([NotNull] IModelNode updatedModelNode);

        void ApplyLayout([NotNull] GroupLayoutInfo diagramLayout);
        void ClearDiagram();

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