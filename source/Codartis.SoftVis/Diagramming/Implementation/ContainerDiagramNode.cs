using System;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling.Definition;
using JetBrains.Annotations;

namespace Codartis.SoftVis.Diagramming.Implementation
{
    /// <summary>
    /// Abstract base for diagram nodes that can contain other diagram nodes. Immutable.
    /// </summary>
    public sealed class ContainerDiagramNode : DiagramNode, IContainerDiagramNode
    {
        public ILayoutGroup LayoutGroup { get; }

        public ContainerDiagramNode([NotNull] IModelNode modelNode, ModelNodeId? parentNodeId = null)
            : this(
                modelNode,
                rect: Rect2D.Undefined,
                addedAt: DateTime.Now,
                parentNodeId,
                Implementation.LayoutGroup.Empty(modelNode.Id))
        {
        }

        public ContainerDiagramNode(
            [NotNull] IModelNode modelNode,
            Rect2D rect,
            DateTime addedAt,
            ModelNodeId? parentNodeId,
            ILayoutGroup layoutGroup)
            : base(modelNode, rect, addedAt, parentNodeId)
        {
            LayoutGroup = layoutGroup;
        }

        public IContainerDiagramNode AddNode(IDiagramNode node, ModelNodeId parentNodeId)
        {
            return (IContainerDiagramNode)CreateInstance(ModelNode, Rect, AddedAt, ParentNodeId, LayoutGroup.AddNode(node, parentNodeId));
        }

        public IContainerDiagramNode UpdateNode(IDiagramNode updatedNode)
        {
            return (IContainerDiagramNode)CreateInstance(ModelNode, Rect, AddedAt, ParentNodeId, LayoutGroup.UpdateNode(updatedNode));
        }

        public IContainerDiagramNode RemoveNode(ModelNodeId nodeId)
        {
            return (IContainerDiagramNode)CreateInstance(ModelNode, Rect, AddedAt, ParentNodeId, LayoutGroup.RemoveNode(nodeId));
        }

        public IContainerDiagramNode AddConnector(IDiagramConnector connector)
        {
            return (IContainerDiagramNode)CreateInstance(ModelNode, Rect, AddedAt, ParentNodeId, LayoutGroup.AddConnector(connector));
        }

        public IContainerDiagramNode UpdateConnector(IDiagramConnector updatedConnector)
        {
            return (IContainerDiagramNode)CreateInstance(ModelNode, Rect, AddedAt, ParentNodeId, LayoutGroup.UpdateConnector(updatedConnector));
        }

        public IContainerDiagramNode RemoveConnector(ModelRelationshipId connectorId)
        {
            return (IContainerDiagramNode)CreateInstance(ModelNode, Rect, AddedAt, ParentNodeId, LayoutGroup.RemoveConnector(connectorId));
        }

        protected override IDiagramNode CreateInstance(
            IModelNode modelNode,
            Rect2D rect,
            DateTime addedAt,
            ModelNodeId? parentNodeId)
        {
            return CreateInstance(modelNode, rect, addedAt, parentNodeId, LayoutGroup);
        }

        [NotNull]
        private static IDiagramNode CreateInstance(
            [NotNull] IModelNode modelNode,
            Rect2D rect,
            DateTime addedAt,
            ModelNodeId? parentNodeId,
            ILayoutGroup layoutGroup)
        {
            return new ContainerDiagramNode(modelNode, rect, addedAt, parentNodeId, layoutGroup);
        }
    }
}