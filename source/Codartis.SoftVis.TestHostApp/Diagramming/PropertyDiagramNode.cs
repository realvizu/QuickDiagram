using System;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Diagramming.Implementation;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.SoftVis.TestHostApp.Modeling;

namespace Codartis.SoftVis.TestHostApp.Diagramming
{
    internal sealed class PropertyDiagramNode : DiagramNodeBase
    {
        public PropertyDiagramNode(PropertyNode modelNode, TypeDiagramNode parentDiagramNode)
            : base(modelNode, parentDiagramNode.Id)
        {
        }

        public PropertyDiagramNode(IModelNode modelNode, Size2D size, Point2D center, DateTime addedAt, ModelNodeId? parentNodeId)
            : base(modelNode, size, center, addedAt, parentNodeId)
        {
        }

        protected override IDiagramNode CreateInstance(IModelNode modelNode, Size2D size, Point2D center, DateTime addedAt, ModelNodeId? parentNodeId)
            => new PropertyDiagramNode(modelNode, size, center, addedAt, parentNodeId);
    }
}