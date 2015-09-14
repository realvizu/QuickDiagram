using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.TestHostApp.TestData
{
    class TestInterface : TestModelEntity
    {
        public TestInterface(string name)
            : base(name, ModelEntityType.Interface)
        {
        }
    }
}
