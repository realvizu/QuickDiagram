using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.TestHostApp.Modeling
{
    internal interface ITestModelMutator : IModelMutator
    {
        void AddItemToCurrentGroup(IModelNode modelNode);
        void StartNewGroup();
    }
}
