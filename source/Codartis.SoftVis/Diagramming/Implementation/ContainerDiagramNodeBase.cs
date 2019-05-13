using System.Collections.Generic;
using System.Collections.Immutable;
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
            : this(modelNode, Size2D.Zero, Point2D.Undefined, ImmutableList<IDiagramNode>.Empty)
        {
        }

        protected ContainerDiagramNodeBase(IModelNode modelNode, Size2D size, Point2D center, ImmutableList<IDiagramNode> childNodes)
            :base(modelNode, size, center, parentDiagramNode :null)
        {
            _childNodes = childNodes;
        }

        public IEnumerable<IDiagramNode> ChildNodes => _childNodes;

        public IDiagramNode AddChildNode(IDiagramNode childNode) 
            => CreateInstance(ModelNode, Size, Center, _childNodes.Add(childNode));

        public IDiagramNode RemoveChildNode(IDiagramNode childNode)
            => CreateInstance(ModelNode, Size, Center, _childNodes.Remove(childNode));

        protected sealed override IDiagramNode CreateInstance(IModelNode modelNode, Size2D size, Point2D center) 
            => CreateInstance(modelNode, size, center, _childNodes);

        protected abstract IDiagramNode CreateInstance(IModelNode modelNode, Size2D size, Point2D center,
            ImmutableList<IDiagramNode> childNodes);
    }
}
