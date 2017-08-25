using Codartis.SoftVis.Diagramming.Implementation;
using Codartis.SoftVis.TestHostApp.Modeling;

namespace Codartis.SoftVis.TestHostApp.Diagramming
{
    internal class TypeDiagramNode : DiagramNode
    {
        public TypeNode TypeNode { get; }

        public TypeDiagramNode(TypeNode typeNode) : base(typeNode)
        {
            TypeNode = typeNode;
        }
    }
}
