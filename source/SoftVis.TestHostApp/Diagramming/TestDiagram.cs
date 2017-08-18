using Codartis.SoftVis.Diagramming.Implementation;
using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.TestHostApp.Diagramming
{
    internal class TestDiagram : AutoArrangingDiagram
    {
        public TestDiagram(IModelProvider modelProvider, DiagramBuilderBase diagramBuilder)
            : base(modelProvider, diagramBuilder, new TestDiagramNodeFactory())
        {
        }
    }
}
