using Codartis.SoftVis.Diagramming.Implementation;
using Codartis.SoftVis.Modeling2;
using Codartis.SoftVis.VisualStudioIntegration.Modeling;

namespace Codartis.SoftVis.VisualStudioIntegration.Diagramming
{
    internal class TypeDiagramNode : DiagramNode
    {
        public IRoslynTypeNode RoslynTypeNode { get; }

        public TypeDiagramNode(IRoslynTypeNode roslynTypeNode) 
            : base(roslynTypeNode)
        {
            RoslynTypeNode = roslynTypeNode;
        }

        public ModelNodeStereotype Stereotype => RoslynTypeNode.Stereotype;
    }
}
