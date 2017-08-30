using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.TestHostApp.Modeling
{
    internal interface ITestModelStore : IModelStore
    {
        TestModel CurrentTestModel { get; }

        void AddItemToCurrentGroup(IModelNode modelNode);
        void StartNewGroup();
    }
}
