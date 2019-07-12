using System;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Diagramming.Implementation;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.UnitTests.TestSubjects
{
    internal sealed class TestDiagramNode : ContainerDiagramNodeBase
    {
        public TestDiagramNode(string name = "dummy", IContainerDiagramNode parentDiagramNode = null)
            : base(new TestModelNode(name), parentDiagramNode)
        {
        }

        private TestDiagramNode(
            IModelNode modelNode,
            Size2D size,
            Point2D center,
            DateTime addedAt,
            IContainerDiagramNode parentDiagramNode,
            ILayoutGroup layoutGroup)
            : base(modelNode, size, center, addedAt, parentDiagramNode, layoutGroup)
        {
        }

        protected override IDiagramNode CreateInstance(
            IModelNode modelNode,
            Size2D size,
            Point2D center,
            DateTime addedAt,
            IContainerDiagramNode parentDiagramNode,
            ILayoutGroup layoutGroup)
        {
            return new TestDiagramNode(modelNode, size, center, addedAt, parentDiagramNode, layoutGroup);
        }
    }
}