using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.Modeling.Implementation;

namespace Codartis.SoftVis.TestHostApp.Modeling
{
    internal class TestModelServiceFactory : IModelServiceFactory
    {
        public IModelService Create()
        {
            return new TestModelService(new ModelStore(), new TestModelRelationshipFactory());
        }
    }
}