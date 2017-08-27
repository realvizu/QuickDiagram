using Codartis.SoftVis.Diagramming;

namespace Codartis.SoftVis.TestHostApp.Diagramming
{
    internal class TestDiagramStoreFactory : IDiagramStoreFactory
    {
        public IDiagramStore Create()
        {
            return new TestDiagramStore(new TestDiagram());
        }
    }
}