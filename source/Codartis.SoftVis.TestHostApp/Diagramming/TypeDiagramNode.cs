using System;
using System.Collections.Immutable;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Diagramming.Implementation;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.TestHostApp.Modeling;

namespace Codartis.SoftVis.TestHostApp.Diagramming
{
    internal sealed class TypeDiagramNode : ContainerDiagramNodeBase
    {
        public TypeDiagramNode(TypeNode typeNode)
            : base(typeNode)
        {
        }

        public TypeDiagramNode(
            TypeNode typeNode,
            Size2D size,
            Point2D center,
            DateTime addedAt,
            IContainerDiagramNode parentDiagramNode,
            ImmutableList<IDiagramNode> childNodes,
            Size2D embeddedDiagramSize)
            : base(typeNode, size, center, addedAt, parentDiagramNode, childNodes, embeddedDiagramSize)
        {
        }

        public TypeNode TypeNode => (TypeNode)ModelNode;

        protected override IDiagramNode CreateInstance(
            IModelNode modelNode,
            Size2D size,
            Point2D center,
            DateTime addedAt,
            IContainerDiagramNode parentDiagramNode,
            ImmutableList<IDiagramNode> childNodes,
            Size2D embeddedDiagramSize)
            => new TypeDiagramNode((TypeNode)modelNode, size, center, addedAt, parentDiagramNode, childNodes, embeddedDiagramSize);
    }
}