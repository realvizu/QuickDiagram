using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Diagramming.Implementation;
using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.TestHostApp.Diagramming
{
    internal class TestDiagramServiceFactory : IDiagramServiceFactory
    {
        public IDiagramService Create(IModelService modelService)
        {
            return new TestDiagramService(
                Diagram.Empty, 
                modelService);
        }
    }
}