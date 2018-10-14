using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.Modeling.Implementation;

namespace Codartis.SoftVis.TestHostApp.Modeling
{
    internal class TestModelStore : ModelStore, ITestModelMutator
    {
        public TestModelStore(TestModel model)
            : base(model)
        {
        }

        public TestModel TestModel => (TestModel)Model;

        public void AddItemToCurrentGroup(IModelNode modelNode)
        {
            lock (ModelUpdateLockObject)
            {
                Model = TestModel.AddItemToCurrentGroup(modelNode);
            }
        }

        public void StartNewGroup()
        {
            lock (ModelUpdateLockObject)
            {
                Model = TestModel.StartNewGroup();
            }
        }
    }
}
