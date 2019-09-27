using System.Collections.Generic;
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

        [NotNull] [ItemNotNull] ISet<IDiagramNode> Nodes { get; }
        [NotNull] [ItemNotNull] ISet<IDiagramConnector> Connectors { get; }

        bool IsEmpty { get; }

        /// <summary>
        /// The absolute position and size of the layout group.
        /// </summary>
        Rect2D Rect { get; }

        [NotNull]
        IDiagramNode GetNode(ModelNodeId nodeId);

        [NotNull]
        IDiagramConnector GetConnector(ModelRelationshipId relationshipId);

        void SetChildrenAreaSize(ModelNodeId nodeId, Size2D childrenAreaSize);
    }
}