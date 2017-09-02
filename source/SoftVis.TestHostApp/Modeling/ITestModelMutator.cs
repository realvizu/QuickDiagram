using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.TestHostApp.Modeling
{
    internal interface ITestModelMutator : IModelMutator
    {
        TestModel TestModel { get; }

        void AddItemToCurrentGroup(IModelNode modelNode);
        void StartNewGroup();
    }
}
