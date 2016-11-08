using Codartis.SoftVis.Diagramming.Implementation;

namespace Codartis.SoftVis.Diagramming.UnitTests.Diagramming.Layout.Incremental.TestSubjects
{
    internal sealed class TestDiagramNode : DiagramNode
    {
        public TestDiagramNode(string name = null) 
            : base(new TestModelEntity(name))
        {
        }
    }
}
