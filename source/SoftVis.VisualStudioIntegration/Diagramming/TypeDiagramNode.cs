using Codartis.SoftVis.Diagramming.Implementation;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.VisualStudioIntegration.Modeling;

namespace Codartis.SoftVis.VisualStudioIntegration.Diagramming
{
    internal sealed class TypeDiagramNode : DiagramNode
    {
        public TypeDiagramNode(IRoslynTypeNode roslynTypeNode) 
            : base(roslynTypeNode)
        {
        }

        public IRoslynTypeNode RoslynTypeNode => (IRoslynTypeNode)ModelItem;
        public ModelNodeStereotype Stereotype => RoslynTypeNode.Stereotype;
    }
}
