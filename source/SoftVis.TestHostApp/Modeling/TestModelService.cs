using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.Modeling.Implementation;

namespace Codartis.SoftVis.TestHostApp.Modeling
{
    internal class TestModelService : ModelService, ITestModelService
    {
        public TestModelService(ITestModelStore modelStore)
            : base(modelStore)
        {
        }

        public TestModel CurrentTestModel => (TestModel) CurrentModel;
        private TestModelStore TestModelStore => (TestModelStore)ModelStore;

        public void AddItemToCurrentGroup(IModelNode modelNode) => TestModelStore.AddItemToCurrentGroup(modelNode);
        public void StartNewGroup() => TestModelStore.StartNewGroup();
    }
}
