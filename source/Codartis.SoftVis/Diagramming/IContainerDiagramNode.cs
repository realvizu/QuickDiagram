using Codartis.SoftVis.Modeling;
using JetBrains.Annotations;

namespace Codartis.SoftVis.Diagramming
{
    /// <summary>
    /// A container node is a diagram node that can have child nodes that form a layout group.
    /// </summary>
    public interface IContainerDiagramNode : IDiagramNode
    {
        ILayoutGroup LayoutGroup { get; }

        [NotNull]
        IContainerDiagramNode AddNode([NotNull] IDiagramNode node, ModelNodeId parentNodeId);

        [NotNull]
        IContainerDiagramNode UpdateNode([NotNull] IDiagramNode updatedNode);

        [NotNull]
        IContainerDiagramNode RemoveNode(ModelNodeId nodeId);

        [NotNull]
        IContainerDiagramNode AddConnector([NotNull] IDiagramConnector connector);

        [NotNull]
        IContainerDiagramNode UpdateConnector([NotNull] IDiagramConnector updatedConnector);

        [NotNull]
        IContainerDiagramNode RemoveConnector(ModelRelationshipId connectorId);
    }
}