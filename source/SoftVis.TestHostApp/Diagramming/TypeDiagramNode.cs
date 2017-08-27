using Codartis.SoftVis.Diagramming.Implementation;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.TestHostApp.Modeling;

namespace Codartis.SoftVis.TestHostApp.Diagramming
{
    internal class TypeDiagramNode : DiagramNode
    {
        public TypeDiagramNode(TypeNode typeNode, Size2D size, Point2D center) 
            : base(typeNode, size, center)
        {
        }

        public TypeDiagramNode(TypeNode typeNode) 
            : base(typeNode)
        {
        }

        public TypeNode TypeNode => (TypeNode)ModelNode;

        protected override DiagramNode CreateInstance(IModelNode modelNode, Size2D size, Point2D center) 
            => new TypeDiagramNode((TypeNode)modelNode, size, center);
    }
}
