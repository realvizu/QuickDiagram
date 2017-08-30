using Codartis.SoftVis.Diagramming.Implementation;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.VisualStudioIntegration.Modeling;

namespace Codartis.SoftVis.VisualStudioIntegration.Diagramming
{
    internal sealed class RoslynTypeDiagramNode : DiagramNode
    {
        public RoslynTypeDiagramNode(IRoslynTypeNode roslynTypeNode) 
            : this(roslynTypeNode, Size2D.Undefined, Point2D.Undefined)
        {
        }

        public RoslynTypeDiagramNode(IRoslynTypeNode roslynTypeNode, Size2D size, Point2D center)
            : base(roslynTypeNode, size, center)
        {
        }

        public IRoslynTypeNode RoslynTypeNode => (IRoslynTypeNode)ModelNode;
        public bool IsAbstract => RoslynTypeNode.IsAbstract;

        protected override DiagramNode CreateInstance(IModelNode modelNode, Size2D size, Point2D center) 
            => new RoslynTypeDiagramNode((IRoslynTypeNode)modelNode, size, center);
    }
}
