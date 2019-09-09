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

        IDiagramNode AddNode(ModelNodeId nodeId, ModelNodeId? parentNodeId = null);
        void RemoveNode(ModelNodeId nodeId);
        IDiagramConnector AddConnector(ModelRelationshipId relationshipId);
        void RemoveConnector(ModelRelationshipId relationshipId);

        void UpdateModel([NotNull] IModel model);
        void UpdateModelNode([NotNull] IModelNode updatedModelNode);

        void UpdateDiagramNodeSize(IDiagramNode diagramNode, Size2D newSize);
        void UpdateDiagramNodeCenter(IDiagramNode diagramNode, Point2D newCenter);
        void UpdateDiagramNodeTopLeft(IDiagramNode diagramNode, Point2D newTopLeft);
        void UpdateConnectorRoute(ModelRelationshipId relationshipId, Route newRoute);
        void ClearDiagram();

        [NotNull]
        [ItemNotNull]
        IReadOnlyList<IDiagramNode> AddNodes(
            [NotNull] IEnumerable<ModelNodeId> modelNodeIds,
            CancellationToken cancellationToken = default,
            IIncrementalProgress progress = null);

        ConnectorType GetConnectorType(ModelRelationshipStereotype stereotype);
    }
}