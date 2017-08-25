using Codartis.SoftVis.Diagramming.Implementation;
using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.TestHostApp.Diagramming
{
    internal class TestDiagram : AutoArrangingDiagram
    {
        public TestDiagram(IReadOnlyModelStore modelProvider, DiagramStore diagramStore)
            : base(modelProvider, diagramStore, new TestDiagramShapeFactory())
        {
        }
    }
}
