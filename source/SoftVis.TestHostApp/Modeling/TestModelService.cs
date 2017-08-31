using Codartis.SoftVis.Modeling.Implementation;

namespace Codartis.SoftVis.TestHostApp.Modeling
{
    internal class TestModelService : ModelService, ITestModelService
    {
        public TestModelService(ITestModelStore modelStore)
            : base(modelStore)
        {
        }

        public TestModelStore TestModelStore => (TestModelStore)ModelStore;
    }
}
