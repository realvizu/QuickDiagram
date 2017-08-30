using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.Modeling.Implementation;

namespace Codartis.SoftVis.TestHostApp.Modeling
{
    internal class TestModelStore : ModelStore, ITestModelStore
    {
        public TestModelStore(TestModel model)
            : base(model)
        {
        }

        public TestModel CurrentTestModel => (TestModel)CurrentModel;

        public void AddItemToCurrentGroup(IModelNode modelNode)
        {
            lock (ModelUpdateLockObject)
            {
                CurrentModel = CurrentTestModel.AddItemToCurrentGroup(modelNode);
            }
        }

        public void StartNewGroup()
        {
            lock (ModelUpdateLockObject)
            {
                CurrentModel = CurrentTestModel.StartNewGroup();
            }
        }
    }
}
