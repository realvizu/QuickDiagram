using System;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Diagramming.Implementation;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.VisualStudioIntegration.Modeling;

namespace Codartis.SoftVis.VisualStudioIntegration.Diagramming
{
    /// <summary>
    /// An immutable diagram node that represents a Roslyn type.
    /// </summary>
    internal sealed class RoslynTypeDiagramNode : ContainerDiagramNodeBase
    {
        public RoslynTypeDiagramNode(IRoslynTypeNode roslynTypeNode)
            : base(roslynTypeNode)
        {
        }

        public RoslynTypeDiagramNode(
            IRoslynTypeNode roslynTypeNode,
            Size2D size,
            Point2D center,
            DateTime addedAt,
            IContainerDiagramNode parentDiagramNode,
            ILayoutGroup layoutGroup)
            : base(roslynTypeNode, size, center, addedAt, parentDiagramNode, layoutGroup)
        {
        }

        public IRoslynTypeNode RoslynTypeNode => (IRoslynTypeNode)ModelNode;
        public bool IsAbstract => RoslynTypeNode.IsAbstract;

        protected override IDiagramNode CreateInstance(
            IModelNode modelNode,
            Size2D size,
            Point2D center,
            DateTime addedAt,
            IContainerDiagramNode parentDiagramNode,
            ILayoutGroup layoutGroup)
            => new RoslynTypeDiagramNode((IRoslynTypeNode)modelNode, size, center, addedAt, parentDiagramNode, layoutGroup);
    }
}