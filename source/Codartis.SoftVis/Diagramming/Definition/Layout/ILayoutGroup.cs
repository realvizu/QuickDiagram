using System.Collections.Generic;
using Codartis.SoftVis.Modeling.Definition;
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
        [NotNull] [ItemNotNull] ISet<IDiagramNode> Nodes { get; }
        [NotNull] [ItemNotNull] ISet<IDiagramConnector> Connectors { get; }

        bool IsEmpty { get; }

        [NotNull]
        IDiagramNode GetNode(ModelNodeId nodeId);

        [NotNull]
        IDiagramConnector GetConnector(ModelRelationshipId relationshipId);
    }
}