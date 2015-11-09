using Codartis.SoftVis.Geometry;

namespace Codartis.SoftVis.Diagramming.UnitTests.Diagramming.Layout.Incremental.TestSubjects
{
    internal sealed class TestDiagramNode : DiagramNode
    {
        public TestDiagramNode() 
            : base(new TestModelEntity(), Point2D.Empty, Size2D.Zero)
        {
        }
    }
}
