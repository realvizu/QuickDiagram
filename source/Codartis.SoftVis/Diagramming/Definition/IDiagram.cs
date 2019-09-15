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
    /// A diagram consists of layout groups that act like little diagrams that the main diagram is composed of.
    /// Each layout group consists of shapes that represent model items: diagram nodes for model nodes and diagram connectors for model relationships.
    /// A connector belongs to a layout group if both its source and target nodes are in that group.
    /// Those connectors whose source and target nodes belong to different layout groups form a special connector group:
    /// the CrossLayoutGroupConnectors, that have different layout rules than other layout groups.
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

        /// <summary>
        /// Gets the root level layout group of the diagram.
        /// </summary>
        [NotNull]
        ILayoutGroup RootLayoutGroup { get; }

        /// <summary>
        /// Gets the layout group of a given node.
        /// </summary>
        Maybe<ILayoutGroup> GetLayoutGroupByNodeId(ModelNodeId modelNodeId);

        /// <summary>
        /// Returns those connectors that cross between layout groups therefore doesn't belong to any of them.
        /// </summary>
        [NotNull]
        [ItemNotNull]
        IImmutableSet<IDiagramConnector> CrossLayoutGroupConnectors { get; }

        /// <remarks>
        /// This should remove all shapes whose model ID does not exist in the new model.
        /// </remarks>
        [NotNull]
        IDiagram WithModel([NotNull] IModel newModel);

        [NotNull]
        IDiagram AddNode([NotNull] IDiagramNode newNode);

        [NotNull]
        IDiagram UpdateNode([NotNull] IDiagramNode updatedNode);

        [NotNull]
        IDiagram UpdateNodes([NotNull] [ItemNotNull] IEnumerable<IDiagramNode> updatedNodes);

        [NotNull]
        IDiagram RemoveNode(ModelNodeId nodeId);

        [NotNull]
        IDiagram AddConnector([NotNull] IDiagramConnector newConnector);

        [NotNull]
        IDiagram UpdateConnector([NotNull] IDiagramConnector updatedConnector);

        [NotNull]
        IDiagram UpdateConnectors([NotNull] [ItemNotNull] IEnumerable<IDiagramConnector> updatedConnectors);

        [NotNull]
        IDiagram RemoveConnector(ModelRelationshipId relationshipId);

        [NotNull]
        IDiagram Clear();

        bool NodeExists(ModelNodeId modelNodeId);
        bool ConnectorExists(ModelRelationshipId modelRelationshipId);
        bool PathExists(ModelNodeId sourceModelNodeId, ModelNodeId targetModelNodeId);
        bool PathExists(Maybe<ModelNodeId> maybeSourceModelNodeId, Maybe<ModelNodeId> maybeTargetModelNodeId);
        bool IsConnectorRedundant(ModelRelationshipId modelRelationshipId);
        bool IsCrossingLayoutGroups(ModelRelationshipId modelRelationshipId);

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