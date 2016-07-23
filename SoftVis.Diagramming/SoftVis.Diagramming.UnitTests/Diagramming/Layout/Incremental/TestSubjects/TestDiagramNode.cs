using Codartis.SoftVis.Diagramming.Implementation;
using Codartis.SoftVis.Geometry;

namespace Codartis.SoftVis.Diagramming.UnitTests.Diagramming.Layout.Incremental.TestSubjects
{
    internal sealed class TestDiagramNode : DiagramNode
    {
        public TestDiagramNode(string name = null) 
            : base(new TestModelEntity(name), Point2D.Empty, Size2D.Zero)
        {
        }
    }
}
