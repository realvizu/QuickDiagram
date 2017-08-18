using Codartis.SoftVis.Diagramming.Implementation;

namespace Codartis.SoftVis.Diagramming.UnitTests.TestSubjects
{
    internal sealed class TestDiagramNode : DiagramNode
    {
        public TestDiagramNode(string name = "dummy") 
            : base(new TestModelNode(name))
        {
        }
    }
}
