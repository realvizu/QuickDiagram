using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.TestHostApp.TestData
{
    class TestClass : TestModelEntity
    {
        public TestClass(string name, TestModelEntity baseEntity = null)
            : base(name, ModelEntityType.Class, baseEntity)
        {
        }
    }
}
