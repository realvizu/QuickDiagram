using System;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.Diagramming.Implementation
{
    /// <summary>
    /// Abstract base for diagram nodes that can contain other diagram nodes. Immutable.
    /// </summary>
    public abstract class ContainerDiagramNodeBase : DiagramNodeBase, IContainerDiagramNode
    {
        public ILayoutGroup LayoutGroup { get; }

        protected ContainerDiagramNodeBase(IModelNode modelNode, ModelNodeId? parentNodeId = null)
            : this(
                modelNode,
                size: Size2D.Zero,
                center: Point2D.Undefined,
                addedAt: DateTime.Now,
                parentNodeId,
                Implementation.LayoutGroup.Empty(modelNode.Id))
        {
        }

        protected ContainerDiagramNodeBase(
            IModelNode modelNode,
            Size2D size,
            Point2D center,
            DateTime addedAt,
            ModelNodeId? parentNodeId,
            ILayoutGroup layoutGroup)
            : base(modelNode, size, center, addedAt, parentNodeId)
        {
            LayoutGroup = layoutGroup;
        }

        public IContainerDiagramNode AddNode(IDiagramNode node, ModelNodeId parentNodeId)
        {
            return (IContainerDiagramNode)CreateInstance(ModelNode, Size, Center, AddedAt, ParentNodeId, LayoutGroup.AddNode(node, parentNodeId));
        }

        public IContainerDiagramNode UpdateNode(IDiagramNode updatedNode)
        {
            return (IContainerDiagramNode)CreateInstance(ModelNode, Size, Center, AddedAt, ParentNodeId, LayoutGroup.UpdateNode(updatedNode));
        }

        public IContainerDiagramNode RemoveNode(ModelNodeId nodeId)
        {
            return (IContainerDiagramNode)CreateInstance(ModelNode, Size, Center, AddedAt, ParentNodeId, LayoutGroup.RemoveNode(nodeId));
        }

        public IContainerDiagramNode AddConnector(IDiagramConnector connector)
        {
            return (IContainerDiagramNode)CreateInstance(ModelNode, Size, Center, AddedAt, ParentNodeId, LayoutGroup.AddConnector(connector));
        }

        public IContainerDiagramNode RemoveConnector(ModelRelationshipId connectorId)
        {
            return (IContainerDiagramNode)CreateInstance(ModelNode, Size, Center, AddedAt, ParentNodeId, LayoutGroup.RemoveConnector(connectorId));
        }

        protected sealed override IDiagramNode CreateInstance(
            IModelNode modelNode,
            Size2D size,
            Point2D center,
            DateTime addedAt,
            ModelNodeId? parentNodeId)
        {
            return CreateInstance(modelNode, size, center, addedAt, parentNodeId, LayoutGroup);
        }

        protected abstract IDiagramNode CreateInstance(
            IModelNode modelNode,
            Size2D size,
            Point2D center,
            DateTime addedAt,
            ModelNodeId? parentNodeId,
            ILayoutGroup layoutGroup);
    }
}