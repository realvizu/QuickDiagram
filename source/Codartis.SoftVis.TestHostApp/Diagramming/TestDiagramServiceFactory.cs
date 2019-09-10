using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.Diagramming.Implementation;
using Codartis.SoftVis.Modeling.Definition;

namespace Codartis.SoftVis.TestHostApp.Diagramming
{
    public sealed class TestDiagramServiceFactory : IDiagramServiceFactory
    {
        public IDiagramService Create(IModelService modelService)
        {
            return new DiagramService(modelService.LatestModel, new TestConnectorTypeResolver());
        }
    }
}
