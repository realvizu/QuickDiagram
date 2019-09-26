using System.Collections.Immutable;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.Util;
using JetBrains.Annotations;

namespace Codartis.SoftVis.Diagramming.Definition.Layout
{
    /// <summary>
    /// A part of a diagram that is laid out together.
    /// Consists of diagram nodes and connectors that form a directed graph.
    /// Can have a nodeId that directly contains the group.
    /// </summary>
    public interface ILayoutGroup
    {
        Maybe<ModelNodeId> ContainerNodeId { get; }

        [NotNull] [ItemNotNull] IImmutableSet<IDiagramNode> Nodes { get; }
        [NotNull] [ItemNotNull] IImmutableSet<IDiagramConnector> Connectors { get; }

        bool IsEmpty { get; }

        /// <summary>
        /// The absolute position and size of the layout group.
        /// </summary>
        Rect2D Rect { get; }

        [NotNull]
        IDiagramNode GetNode(ModelNodeId nodeId);

        [NotNull]
        IDiagramConnector GetConnector(ModelRelationshipId relationshipId);
    }
}