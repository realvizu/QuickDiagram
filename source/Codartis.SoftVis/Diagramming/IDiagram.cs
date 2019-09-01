using System.Collections.Generic;
using System.Collections.Immutable;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.Util;
using JetBrains.Annotations;

namespace Codartis.SoftVis.Diagramming
{
    /// <summary>
    /// A diagram is a partial, graphical representation of a model.
    /// Immutable.
    /// </summary>
    /// <remarks>
    /// A diagram shows a subset of the model and there can be many diagrams depicting different areas/aspects of the same model.
    /// A diagram consists consists of layout groups that act like little diagrams that the main diagram is composed of.
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
        IImmutableSet<IDiagramNode> Nodes { get; }

        /// <summary>
        /// Gets all connectors in the diagram.
        /// </summary>
        [NotNull]
        IImmutableSet<IDiagramConnector> Connectors { get; }

        /// <summary>
        /// Gets the root level layout group of the diagram.
        /// </summary>
        [NotNull]
        ILayoutGroup RootLayoutGroup { get; }

        /// <summary>
        /// Returns those connectors that cross between layout groups therefore doesn't belong to any of them.
        /// </summary>
        [NotNull]
        IImmutableSet<IDiagramConnector> CrossLayoutGroupConnectors { get; }

        [NotNull]
        IDiagram WithModel([NotNull] IModel newModel);

        [NotNull]
        IDiagram AddNode([NotNull] IDiagramNode node, ModelNodeId? parentNodeId = null);

        [NotNull]
        IDiagram UpdateNode([NotNull] IDiagramNode updatedNode);

        [NotNull]
        IDiagram RemoveNode(ModelNodeId nodeId);

        [NotNull]
        IDiagram AddConnector([NotNull] IDiagramConnector connector);

        [NotNull]
        IDiagram UpdateConnector([NotNull] IDiagramConnector connector);

        [NotNull]
        IDiagram RemoveConnector(ModelRelationshipId connectorId);

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

        Maybe<IContainerDiagramNode> TryGetContainerNode([NotNull] IDiagramNode diagramNode);

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