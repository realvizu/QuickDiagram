using System;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Diagramming.Implementation;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.SoftVis.UnitTests.Modeling;

namespace Codartis.SoftVis.UnitTests.Diagramming
{
    internal sealed class TestDiagramNode : ContainerDiagramNodeBase
    {
        public TestDiagramNode(string name = "dummy")
            : base(new TestModelNode(name))
        {
        }

        private TestDiagramNode(
            IModelNode modelNode,
            Size2D size,
            Point2D center,
            DateTime addedAt,
            ModelNodeId? parentNodeId,
            ILayoutGroup layoutGroup)
            : base(modelNode, size, center, addedAt, parentNodeId, layoutGroup)
        {
        }

        protected override IDiagramNode CreateInstance(
            IModelNode modelNode,
            Size2D size,
            Point2D center,
            DateTime addedAt,
            ModelNodeId? parentNodeId,
            ILayoutGroup layoutGroup)
        {
            return new TestDiagramNode(modelNode, size, center, addedAt, parentNodeId, layoutGroup);
        }
    }
}