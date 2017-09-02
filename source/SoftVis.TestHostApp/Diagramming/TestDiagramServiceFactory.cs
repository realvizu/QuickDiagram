using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.TestHostApp.Diagramming
{
    internal class TestDiagramServiceFactory : IDiagramServiceFactory
    {
        public IDiagramService Create(IModelService modelService)
        {
            return new TestDiagramService(
                new TestDiagram(), 
                modelService,
                new TestDiagramShapeFactory());
        }
    }
}