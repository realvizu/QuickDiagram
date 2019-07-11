using System.Collections.Generic;
using System.Collections.Immutable;
using Codartis.SoftVis.Modeling;
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
    /// A connector belongs to a layout group id both its source and target nodes are in that group.
    /// Those connectors whose source and target nodes belong to different layout groups form a special connector group:
    /// the CrossLayoutGroupConnectors, that have different layout rules than other layout groups.
    /// </remarks>
    public interface IDiagram
    {
        /// <summary>
        /// Gets all nodes in the diagram.
        /// </summary>
        IImmutableSet<IDiagramNode> Nodes { get; }

        /// <summary>
        /// Gets all connectors in the diagram.
        /// </summary>
        IImmutableSet<IDiagramConnector> Connectors { get; }

        /// <summary>
        /// Gets the root level layout group of the diagram.
        /// </summary>
        ILayoutGroup RootLayoutGroup { get; }

        /// <summary>
        /// Returns those connectors that cross between layout groups therefore doesn't belong to any of them.
        /// </summary>
        IImmutableSet<IDiagramConnector> CrossLayoutGroupConnectors { get; }

        bool NodeExists(ModelNodeId modelNodeId);
        bool ConnectorExists(ModelRelationshipId modelRelationshipId);
        bool PathExists(ModelNodeId sourceModelNodeId, ModelNodeId targetModelNodeId);
        bool IsConnectorRedundant(ModelRelationshipId modelRelationshipId);

        IDiagramNode GetNode(ModelNodeId modelNodeId);
        bool TryGetNode(ModelNodeId modelNodeId, out IDiagramNode node);
        IDiagramConnector GetConnector(ModelRelationshipId modelRelationshipId);
        bool TryGetConnector(ModelRelationshipId modelRelationshipId, out IDiagramConnector connector);
        IEnumerable<IDiagramConnector> GetConnectorsByNode(ModelNodeId id);
        IEnumerable<IDiagramNode> GetAdjacentNodes(ModelNodeId id, DirectedModelRelationshipType? directedModelRelationshipType = null);

        [NotNull]
        IDiagram WithNode([NotNull] IDiagramNode node, ModelNodeId? parentNodeId = null);

        [NotNull]
        IDiagram WithoutNode(ModelNodeId nodeId);

        [NotNull]
        IDiagram WithConnector([NotNull] IDiagramConnector connector);

        [NotNull]
        IDiagram WithoutConnector(ModelRelationshipId connectorId);

        [NotNull]
        IDiagram Clear();
    }
}