using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.TestHostApp.Modeling
{
    internal interface ITestModelStore : IModelStore
    {
        void AddItemToCurrentGroup(IModelNode modelNode);
        void StartNewGroup();
    }
}
