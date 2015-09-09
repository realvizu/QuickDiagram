using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.TestHostApp.TestData
{
    class TestInterface : TestModelEntity
    {
        public TestInterface(string name, TestModelEntity baseEntity = null)
            : base(name, ModelEntityType.Interface, baseEntity)
        {
        }
    }
}
