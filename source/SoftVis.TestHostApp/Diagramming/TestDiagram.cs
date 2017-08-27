using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Diagramming.Implementation;

namespace Codartis.SoftVis.TestHostApp.Diagramming
{
    internal class TestDiagram : Diagram
    {
        public TestDiagram()
            : this(new DiagramGraph())
        {
        }

        public TestDiagram(DiagramGraph diagramGraph)
            : base(diagramGraph)
        {
        }

        protected override IDiagram CreateInstance(DiagramGraph graph)
        {
            return new TestDiagram(graph);
        }
    }
}
