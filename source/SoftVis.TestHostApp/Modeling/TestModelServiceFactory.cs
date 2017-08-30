using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.TestHostApp.Modeling
{
    internal class TestModelServiceFactory : IModelServiceFactory
    {
        public IModelService Create()
        {
            return new TestModelService(new TestModelStore(new TestModel()));
        }
    }
}