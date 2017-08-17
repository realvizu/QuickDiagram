using Codartis.SoftVis.Diagramming.Implementation;
using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.TestHostApp.Diagramming
{
    internal class TestDiagram : AutoArrangingDiagram
    {
        public TestDiagram(IModelBuilder modelBuilder, DiagramBuilderBase diagramBuilder)
            : base(modelBuilder, diagramBuilder, new TestDiagramNodeFactory())
        {
        }
    }
}
