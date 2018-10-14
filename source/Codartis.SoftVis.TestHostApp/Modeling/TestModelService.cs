using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.Modeling.Implementation;

namespace Codartis.SoftVis.TestHostApp.Modeling
{
    internal sealed class TestModelService : ModelServiceBase, ITestModelService
    {
        public TestModelService()
            : base(new TestModelStore(new TestModel()), new TestModelRelationshipFactory())
        {
        }

        private ITestModelMutator TestModelMutator => (ITestModelMutator)ModelStore;

        public TestModel TestModel => (TestModel)Model;

        public void AddItemToCurrentGroup(IModelNode modelNode) => TestModelMutator.AddItemToCurrentGroup(modelNode);
        public void StartNewGroup() => TestModelMutator.StartNewGroup();
    }
}
