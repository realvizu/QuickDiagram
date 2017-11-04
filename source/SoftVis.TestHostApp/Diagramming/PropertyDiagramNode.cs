using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Diagramming.Implementation;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.TestHostApp.Modeling;

namespace Codartis.SoftVis.TestHostApp.Diagramming
{
    internal sealed class PropertyDiagramNode : DiagramNodeBase
    {
        public PropertyDiagramNode(PropertyNode propertyNode) 
            : base(propertyNode)
        {
        }

        public PropertyNode PropertyNode => (PropertyNode)ModelNode;

        protected override IDiagramNode CreateInstance(IModelNode modelNode, Size2D size, Point2D center) 
            => new PropertyDiagramNode((PropertyNode)modelNode);
    }
}
