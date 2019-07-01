using System;
using System.Collections.Immutable;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.Diagramming.Implementation
{
    /// <summary>
    /// Abstract base for diagram nodes that can contain other diagram nodes. Immutable.
    /// </summary>
    [Immutable]
    public abstract class ContainerDiagramNodeBase : DiagramNodeBase, IContainerDiagramNode
    {
        private readonly ImmutableList<IDiagramNode> _childNodes;

        protected ContainerDiagramNodeBase(IModelNode modelNode)
            : this(modelNode, Size2D.Zero, Point2D.Undefined, DateTime.Now, ImmutableList<IDiagramNode>.Empty)
        {
        }

        protected ContainerDiagramNodeBase(IModelNode modelNode, Size2D size, Point2D center, DateTime addedAt, ImmutableList<IDiagramNode> childNodes)
            : base(modelNode, size, center, addedAt, parentDiagramNode: null)
        {
            _childNodes = childNodes;
        }

        public IImmutableList<IDiagramNode> ChildNodes => _childNodes;

        public IDiagramNode AddChildNode(IDiagramNode childNode)
            => CreateInstance(ModelNode, Size, Center, AddedAt, _childNodes.Add(childNode));

        public IDiagramNode RemoveChildNode(IDiagramNode childNode)
            => CreateInstance(ModelNode, Size, Center, AddedAt, _childNodes.Remove(childNode));

        protected sealed override IDiagramNode CreateInstance(IModelNode modelNode, Size2D size, Point2D center, DateTime addedAt, IContainerDiagramNode parentDiagramNode)
            => CreateInstance(modelNode, size, center, addedAt, _childNodes);

        protected abstract IDiagramNode CreateInstance(IModelNode modelNode, Size2D size, Point2D center, DateTime addedAt, ImmutableList<IDiagramNode> childNodes);
    }
}