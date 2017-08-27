using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.TestHostApp.Modeling
{
    internal class TestModelStoreFactory : IModelStoreFactory
    {
        public IModelStore Create()
        {
            return new TestModelStore(new TestModel());
        }
    }
}