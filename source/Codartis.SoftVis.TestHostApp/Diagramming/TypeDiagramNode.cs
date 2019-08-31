using System;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Diagramming.Implementation;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.SoftVis.TestHostApp.Modeling;

namespace Codartis.SoftVis.TestHostApp.Diagramming
{
    internal sealed class TypeDiagramNode : ContainerDiagramNodeBase
    {
        public TypeDiagramNode(TypeNodeBase typeNode)
            : base(typeNode)
        {
        }

        public TypeDiagramNode(
            TypeNodeBase typeNode,
            Size2D size,
            Point2D center,
            DateTime addedAt,
            ModelNodeId? parentNodeId,
            ILayoutGroup layoutGroup)
            : base(typeNode, size, center, addedAt, parentNodeId, layoutGroup)
        {
        }

        public TypeNodeBase TypeNode => (TypeNodeBase)ModelNode;

        protected override IDiagramNode CreateInstance(
            IModelNode modelNode,
            Size2D size,
            Point2D center,
            DateTime addedAt,
            ModelNodeId? parentNodeId,
            ILayoutGroup layoutGroup)
            => new TypeDiagramNode((TypeNodeBase)modelNode, size, center, addedAt, parentNodeId, layoutGroup);
    }
}