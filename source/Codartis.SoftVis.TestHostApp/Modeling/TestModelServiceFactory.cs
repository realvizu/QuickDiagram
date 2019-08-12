using Codartis.SoftVis.Modeling.Definition;

namespace Codartis.SoftVis.TestHostApp.Modeling
{
    internal class TestModelServiceFactory : IModelServiceFactory
    {
        public IModelService Create()
        {
            return new TestModelService(new TestModelRelationshipFactory());
        }
    }
}