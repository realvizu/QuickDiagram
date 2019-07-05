using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.Diagramming.Implementation
{
    /// <summary>
    /// Abstract base for diagram nodes that can contain other diagram nodes. Immutable.
    /// </summary>
    public abstract class ContainerDiagramNodeBase : DiagramNodeBase, IContainerDiagramNode
    {
        private readonly ImmutableList<IDiagramNode> _childNodes;

        protected ContainerDiagramNodeBase(IModelNode modelNode)
            : this(
                modelNode,
                size: Size2D.Zero,
                center: Point2D.Undefined,
                addedAt: DateTime.Now,
                parentDiagramNode: null,
                childNodes: ImmutableList<IDiagramNode>.Empty)
        {
        }

        protected ContainerDiagramNodeBase(
            IModelNode modelNode,
            Size2D size,
            Point2D center,
            DateTime addedAt,
            IContainerDiagramNode parentDiagramNode,
            ImmutableList<IDiagramNode> childNodes)
            : base(modelNode, size, center, addedAt, parentDiagramNode)
        {
            _childNodes = childNodes;
        }

        public IEnumerable<IDiagramNode> ChildNodes => _childNodes;

        public Rect2D EmbeddedDiagramRect => _childNodes.Select(i => i.Rect).Union();

        public IDiagramNode WithChildNode(IDiagramNode childNode)
            => CreateInstance(ModelNode, Size, Center, AddedAt, ParentDiagramNode, _childNodes.Add(childNode));

        public IDiagramNode WithoutChildNode(IDiagramNode childNode)
            => CreateInstance(ModelNode, Size, Center, AddedAt, ParentDiagramNode, _childNodes.Remove(childNode));

        protected sealed override IDiagramNode CreateInstance(
            IModelNode modelNode,
            Size2D size,
            Point2D center,
            DateTime addedAt,
            IContainerDiagramNode parentDiagramNode)
        {
            return CreateInstance(modelNode, size, center, addedAt, parentDiagramNode, _childNodes);
        }

        protected abstract IDiagramNode CreateInstance(
            IModelNode modelNode,
            Size2D size,
            Point2D center,
            DateTime addedAt,
            IContainerDiagramNode parentDiagramNode,
            ImmutableList<IDiagramNode> childNodes);
    }
}