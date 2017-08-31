using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Diagramming.Implementation;
using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.TestHostApp.Diagramming
{
    internal class TestDiagramServiceFactory : IDiagramServiceFactory
    {
        public IDiagramService Create(IReadOnlyModelStore modelStore)
        {
            return new DiagramService(
                modelStore,
                new TestDiagramStore(new TestDiagram()), 
                new TestDiagramShapeFactory());
        }
    }
}