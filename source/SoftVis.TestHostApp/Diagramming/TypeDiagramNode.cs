using Codartis.SoftVis.Diagramming.Implementation;
using Codartis.SoftVis.TestHostApp.Modeling;

namespace Codartis.SoftVis.TestHostApp.Diagramming
{
    internal class TypeDiagramNode : DiagramNode
    {
        public TypeNodeBase TypeNodeBase { get; }

        public TypeDiagramNode(TypeNodeBase typeNodeBase) : base(typeNodeBase)
        {
            TypeNodeBase = typeNodeBase;
        }
    }
}
