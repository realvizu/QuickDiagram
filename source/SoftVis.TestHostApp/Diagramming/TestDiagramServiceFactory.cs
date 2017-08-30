using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Diagramming.Implementation;

namespace Codartis.SoftVis.TestHostApp.Diagramming
{
    internal class TestDiagramServiceFactory : IDiagramServiceFactory
    {
        public IDiagramService Create()
        {
            return new DiagramService(
                new TestDiagramStore(new TestDiagram()), 
                new TestDiagramShapeFactory());
        }
    }
}