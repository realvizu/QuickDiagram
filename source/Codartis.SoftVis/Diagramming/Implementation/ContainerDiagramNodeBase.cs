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

        protected ContainerDiagramNodeBase(IModelNode modelNode, IContainerDiagramNode parentDiagramNode = null)
            : this(
                modelNode,
                size: Size2D.Zero,
                center: Point2D.Undefined,
                addedAt: DateTime.Now,
                parentDiagramNode: parentDiagramNode,
                Implementation.LayoutGroup.Empty(modelNode.Id))
        {
        }

        protected ContainerDiagramNodeBase(
            IModelNode modelNode,
            Size2D size,
            Point2D center,
            DateTime addedAt,
            IContainerDiagramNode parentDiagramNode,
            ILayoutGroup layoutGroup)
            : base(modelNode, size, center, addedAt, parentDiagramNode)
        {
            LayoutGroup = layoutGroup;
        }

        public IContainerDiagramNode WithLayoutGroup(ILayoutGroup layoutGroup)
        {
            return (IContainerDiagramNode)CreateInstance(ModelNode, Size, Center, AddedAt, ParentDiagramNode, layoutGroup);
        }

        protected sealed override IDiagramNode CreateInstance(
            IModelNode modelNode,
            Size2D size,
            Point2D center,
            DateTime addedAt,
            IContainerDiagramNode parentDiagramNode)
        {
            return CreateInstance(modelNode, size, center, addedAt, parentDiagramNode, LayoutGroup);
        }

        protected abstract IDiagramNode CreateInstance(
            IModelNode modelNode,
            Size2D size,
            Point2D center,
            DateTime addedAt,
            IContainerDiagramNode parentDiagramNode,
            ILayoutGroup layoutGroup);
    }
}