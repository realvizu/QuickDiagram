using System.Collections.Generic;
using System.Collections.Immutable;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.Util;
using JetBrains.Annotations;

namespace Codartis.SoftVis.Diagramming.Definition
{
    /// <summary>
    /// A diagram is a partial, graphical representation of a model.
    /// It consists of nodes and connectors.
    /// Nodes can contain other nodes.
    /// Immutable.
    /// </summary>
    /// <remarks>
    /// A diagram shows a subset of the model and there can be many diagrams depicting different areas/aspects of the same model.
    /// </remarks>
    public interface IDiagram
    {
        /// <summary>
        /// The underlying model. Diagram shapes represent model items.
        /// </summary>
        [NotNull]
        IModel Model { get; }

        /// <summary>
        /// Gets all nodes in the diagram.
        /// </summary>
        [NotNull]
        [ItemNotNull]
        IImmutableSet<IDiagramNode> Nodes { get; }

        /// <summary>
        /// Gets all connectors in the diagram.
        /// </summary>
        [NotNull]
        [ItemNotNull]
        IImmutableSet<IDiagramConnector> Connectors { get; }

        Rect2D Rect { get; }

        bool IsEmpty { get; }

        DiagramChangedEvent AddNode(ModelNodeId nodeId, ModelNodeId? parentNodeId = null);
        DiagramChangedEvent UpdateNodePayloadAreaSize(ModelNodeId nodeId, Size2D newSize);
        DiagramChangedEvent UpdateNodeChildrenAreaSize(ModelNodeId nodeId, Size2D newSize);
        DiagramChangedEvent UpdateNodeCenter(ModelNodeId nodeId, Point2D newCenter);
        DiagramChangedEvent UpdateNodeTopLeft(ModelNodeId nodeId, Point2D newTopLeft);
        DiagramChangedEvent RemoveNode(ModelNodeId nodeId);

        DiagramChangedEvent AddConnector(ModelRelationshipId relationshipId);
        DiagramChangedEvent UpdateConnectorRoute(ModelRelationshipId relationshipId, Route newRoute);
        DiagramChangedEvent RemoveConnector(ModelRelationshipId relationshipId);

        /// <remarks>
        /// This should remove all shapes whose model ID does not exist in the new model.
        /// </remarks>
        DiagramChangedEvent UpdateModel([NotNull] IModel newModel);
        DiagramChangedEvent UpdateModelNode([NotNull] IModelNode updatedModelNode);

        DiagramChangedEvent ApplyLayout(DiagramLayoutInfo diagramLayout);
        DiagramChangedEvent Clear();

        bool NodeExists(ModelNodeId modelNodeId);
        bool ConnectorExists(ModelRelationshipId modelRelationshipId);
        bool PathExists(ModelNodeId sourceModelNodeId, ModelNodeId targetModelNodeId);
        bool PathExists(Maybe<ModelNodeId> maybeSourceModelNodeId, Maybe<ModelNodeId> maybeTargetModelNodeId);
        bool IsConnectorRedundant(ModelRelationshipId modelRelationshipId);

        [NotNull]
        IDiagramNode GetNode(ModelNodeId modelNodeId);

        Maybe<IDiagramNode> TryGetNode(ModelNodeId modelNodeId);

        Rect2D GetRect([NotNull] IEnumerable<ModelNodeId> modelNodeIds);

        [NotNull]
        IDiagramConnector GetConnector(ModelRelationshipId modelRelationshipId);

        Maybe<IDiagramConnector> TryGetConnector(ModelRelationshipId modelRelationshipId);

        [NotNull]
        [ItemNotNull]
        IEnumerable<IDiagramConnector> GetConnectorsByNode(ModelNodeId id);

        //IEnumerable<IDiagramNode> GetAdjacentNodes(ModelNodeId id, DirectedModelRelationshipType? directedModelRelationshipType = null);
    }
}