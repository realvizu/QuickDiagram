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

        bool NodeExists(ModelNodeId modelNodeId);
        bool ConnectorExists(ModelRelationshipId modelRelationshipId);
        bool PathExists(ModelNodeId sourceModelNodeId, ModelNodeId targetModelNodeId, ModelRelationshipStereotype stereotype);
        bool PathExists(Maybe<ModelNodeId> maybeSourceModelNodeId, Maybe<ModelNodeId> maybeTargetModelNodeId, ModelRelationshipStereotype stereotype);
        bool IsConnectorRedundant(ModelRelationshipId modelRelationshipId, ModelRelationshipStereotype stereotype);

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

        [NotNull]
        [ItemNotNull]
        IEnumerable<IDiagramNode> GetChildNodes(ModelNodeId diagramNodeId);
    }
}